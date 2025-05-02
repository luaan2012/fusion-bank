using fusion.bank.central.domain.Interfaces;
using fusion.bank.central.domain.Model;
using fusion.bank.central.domain.Request;
using fusion.bank.central.Request;
using fusion.bank.core.Messages.DataContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.central.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]

    public class CentralBankController(IBankRepository bankRepository, ILogger<CentralBankController> logger) : MainController
    {
        [HttpPost("create-bank")]
        public async Task<IActionResult> CreateBank(BankRequest bankRequest)
        {
            var bank = new Bank();
            bank.CreateBank(bankRequest);
            await bankRepository.SaveBank(bank);

            return CreateResponse(new DataContractMessage<string> { Success = true }, "Banco criado com sucesso");
        }

        [HttpGet("list-all-banks")]
        public async Task<IActionResult> ListAllBanks()
        {
            return CreateResponse(new DataContractMessage<IEnumerable<Bank>> { Data = await bankRepository.ListAllBank(), Success = true });
        }

        [HttpGet("get-bank/id:guid")]
        public async Task<IActionResult> GetBankId(Guid id)
        {
            return CreateResponse(new DataContractMessage<Bank> { Data = await bankRepository.ListBankById(id), Success = true });
        }

        [HttpPut("edit-bank/id:guid")]
        public async Task<IActionResult> GetBankId(Guid id, BankEditRequest bankEditRequest)
        {
            return CreateResponse(new DataContractMessage<Bank> { Data = await bankRepository.UpdateBank(id, bankEditRequest), Success = true });
        }

        [HttpDelete("delete-bank/id:guid")]
        public async Task<IActionResult> DeleteBank (Guid id)
        {
            return CreateResponse(new DataContractMessage<bool> { Data = await bankRepository.DeleteBank(id), Success = true });
        }
    }
}
