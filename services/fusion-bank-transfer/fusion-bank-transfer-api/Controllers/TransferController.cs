using fusion.bank.core;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.transfer.domain;
using fusion.bank.transfer.domain.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.transfer.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController(ITransferRepository transferRepository, 
        IRequestClient<NewTransferAccountRequest> requestClient, IPublishEndpoint publishEndpoint) : MainController
    {
        [HttpPost("create-transfer")]
        public async Task<IActionResult> CreateTransfer(Transfer transfer)
        {
            if (transfer.IsSchedule)
            {
                await transferRepository.SaveTransfer(transfer);

                await publishEndpoint.Publish(GenerateEvent.CreateTransferMadeEvent(transfer.AccountId.ToString(), transfer.Amount, transfer.NameReceive));

                return CreateResponse(new DataContractMessage<Transfer>() { Data = transfer, Success = false });
            }

            var response = await requestClient.GetResponse<DataContractMessage<TransferredAccountResponse>>(new NewTransferAccountRequest(transfer.TransferType, transfer.KeyAccount, transfer.Amount, transfer.AccountNumberOwner));

            if(response.Message.Success)
            {
                transfer.TransferStatus = domain.Enum.TransferStatus.COMPLETE;

                await transferRepository.SaveTransfer(transfer);

                await publishEndpoint.Publish(GenerateEvent.CreateTransferMadeEvent(transfer.AccountId.ToString(), transfer.Amount, transfer.NameReceive));

                await publishEndpoint.Publish(GenerateEvent.CreateTransferReceivedEvent(transfer.AccountId.ToString(), transfer.Amount, transfer.NameOwner));

                return CreateResponse(response.Message, "transferencia realizada com sucesso.");
            }

            return CreateResponse(response.Message);
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
    }
}
