using fusion.bank.transfer.domain;
using fusion.bank.transfer.domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.transfer.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController(ITransferRepository transferRepository) : ControllerBase
    {
        [HttpPost("create-transfer")]
        public async Task<IActionResult> CreateTransfer(Transfer transfer)
        {
            if (transfer.IsSchedule)
            {
                await transferRepository.SaveTransfer(transfer);
                return Ok(transfer);
            }

            return Ok(null);
        }
    }
}
