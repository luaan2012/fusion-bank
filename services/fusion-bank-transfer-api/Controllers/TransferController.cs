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
    public class TransferController(ITransferRepository transferRepository, IRequestClient<NewTransferAccountRequest> requestClient) : ControllerBase
    {
        [HttpPost("create-transfer")]
        public async Task<IActionResult> CreateTransfer(Transfer transfer)
        {
            if (transfer.IsSchedule)
            {
                await transferRepository.SaveTransfer(transfer);
                return Ok(transfer);
            }

            var response = await requestClient.GetResponse<TransferredAccountResponse>(new NewTransferAccountRequest(transfer.TransferType, transfer.KeyAccount, transfer.Amount));

            if(response.Message.Transferred)
            {
                transfer.TransferStatus = domain.Enum.TransferStatus.COMPLETE;
                await transferRepository.SaveTransfer(transfer);

                return Ok("Transferencia realizada com sucesso");
            }

            return BadRequest("Erro ao tentar realizar sua transferencia, tente novamente em segundos.");
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
