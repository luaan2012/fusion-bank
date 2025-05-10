using fusion.bank.core;
using fusion.bank.core.Enum;
using fusion.bank.core.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.transfer.domain;
using fusion.bank.transfer.domain.Enum;
using fusion.bank.transfer.domain.Interfaces;
using fusion.bank.transfer.domain.Request;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.transfer.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController(ITransferRepository transferRepository,
        IRequestClient<NewTransferAccountRequest> requestClient, IPublishEndpoint publishEndpoint,
        IBackgroundTaskQueue backgroundTaskQueue) : MainController
    {
        [HttpPost("create-transfer")]
        public async Task<IActionResult> CreateTransfer(TransferRequest transferRequest)
        {
            var transfer = new Transfer();

            transfer.CreateTransfer(transferRequest);   

            if (transfer.IsSchedule)
            {
                await transferRepository.SaveTransfer(transfer);

                await publishEndpoint.Publish(GenerateEvent.CreateTransferMadeEvent(transfer.AccountId.ToString(), transfer.Amount, transfer.NameReceiver, transfer.TransferType));

                return CreateResponse(new DataContractMessage<Transfer>() { Data = transfer, Success = false });
            }

            if (transfer is { TransferType: TransferType.DOC } or { TransferType: TransferType.TED })
            {
                await publishEndpoint.Publish(GenerateEvent.CreateTransferProcessingEvent(transfer.AccountId.ToString(), transfer.Amount, transfer.NameReceiver, transfer.TransferType));

                await backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (cancellationToken) =>
                {
                    await Task.Delay(
                        transfer.TransferType == TransferType.TED
                            ? Random.Shared.Next(15000, 20000)
                            : Random.Shared.Next(55000, 60000)
                    );
                    
                    await HandleTransfer(transfer);
                });

                return CreateResponse(new DataContractMessage<Transfer>() { Data = transfer, Success = true }, "Transferencia entrou em processamento, aguarde entre 10 a 50 segundos para efetivaçao.");
            }

            return await HandleTransfer(transfer);
        }

        [HttpGet("list-all-transfer")]
        public async Task<IActionResult> ListAllTransfer()
        {
            return Ok(await transferRepository.ListAllTransfers());
        }

        [HttpGet("list-transfer-by-id/id:guid")]
        public async Task<IActionResult> ListTransferById(Guid id)
        {
            return Ok(await transferRepository.ListById(id));
        }

        private async Task<IActionResult> HandleTransfer(Transfer transfer)
        {
            var response = await requestClient.GetResponse<DataContractMessage<TransferredAccountResponse>>(new NewTransferAccountRequest(transfer.TransferType, 
                transfer.KeyAccount, transfer.Amount, transfer.AccountNumberPayer, transfer.AccountNumberReceiver, transfer.AgencyNumberReceiver));

            if(!response.Message.Success && transfer.TransferType != TransferType.PIX)
            {
                await publishEndpoint.Publish(GenerateEvent.CreateTransferFailedEvent(transfer.AccountId.ToString(), transfer.Amount, transfer.NameReceiver, transfer.AccountNumberReceiver, transfer.AgencyNumberReceiver, transfer.TransferType));
            }


            if (response.Message.Success)
            {
                transfer.TransferStatus = TransferStatus.COMPLETE;

                await transferRepository.SaveTransfer(transfer);

                await publishEndpoint.Publish(GenerateEvent.CreateTransferMadeEvent(transfer.AccountId.ToString(), transfer.Amount, transfer.NameReceiver, transfer.TransferType));

                await publishEndpoint.Publish(GenerateEvent.CreateTransferReceivedEvent(response.Message.Data.AccountId.ToString(), transfer.Amount, response.Message.Data.NameReceiver, transfer.TransferType));

                return CreateResponse(response.Message, "transferencia realizada com sucesso.");
            }

            return CreateResponse(response.Message);
        }
    }
}
