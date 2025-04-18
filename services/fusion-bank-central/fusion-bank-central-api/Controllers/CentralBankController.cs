using fusion.bank.central.domain.Interfaces;
using fusion.bank.central.domain.Model;
using fusion.bank.central.Request;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.central.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CentralBankController(IBankRepository bankRepository, ILogger<CentralBankController> logger) : MainController
    {
        [HttpPost("create-bank")]
        public async Task<IActionResult> CreateBank(BankRequest bankRequest)
        {
            var bank = new Bank();
            bank.CreateBank(bankRequest);

            await bankRepository.SaveBank(bank);

            return Ok("Banco criado com sucesso");
        }

        [HttpGet("list-all-banks")]
        public async Task<IActionResult> ListAllBanks()
        {
            return Ok(await bankRepository.ListAllBank());
        }

        [HttpGet("list-all-banks/id:guid")]
        public async Task<IActionResult> ListAllBanks(Guid id)
        {
            return Ok(await bankRepository.ListBankById(id));
        }
    }
}
