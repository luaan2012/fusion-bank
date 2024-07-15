using fusion.bank.central.domain.Interfaces;
using fusion.bank.central.domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.central.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CentralBankController(IBankRepository bankRepository, ILogger<CentralBankController> logger) : ControllerBase
    {
        [HttpPost("create-bank")]
        public async Task<IActionResult> CreateBank(string name, string city, string address, string state)
        {
            var bank = new Bank();
            bank.CreateBank(name, city, address, state);

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
