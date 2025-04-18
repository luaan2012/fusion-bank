using fusion.bank.core.Messages.Producers;
using fusion.bank.deposit.domain.Interfaces;
using fusion.bank.deposit.domain.Models;
using fusion.bank.deposit.domain.Requests;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion_bank_deposit_api.Controllers;

[ApiController]
[Route("[controller]")]
public class DepositController(IDepositRepository depositRepository, ILogger<DepositController> logger, IPublishEndpoint bus) : ControllerBase
{
   
    [HttpPost("generate-billet")]
    public async Task<IActionResult> GenerateBillet(BilletRequest billetRequest)
    {
        var deposit = new Deposit();
        deposit.GenerateCode(billetRequest);

        await depositRepository.SaveDeposit(deposit);

        return Ok(deposit);
    }

    [HttpGet("get-billet-by-id/id:guid")]
    public async Task<IActionResult> GenerateBillet(Guid id)
    {
        return Ok(await depositRepository.GetDepositById(id));
    }

    [HttpGet("list-all-billets")]
    public async Task<IActionResult> ListAllBillets()
    {
        return Ok(await depositRepository.ListAllBillets());
    }

    [HttpGet("deposit/id:guid")]
    public async Task<IActionResult> Depositbillet(Guid id)
    {
        var billet = await depositRepository.GetDepositById(id);

        if(billet is null)
        {
            return BadRequest("Boleto não encontrado");
        }

        //if(billet.Debited)
        //{
        //    return BadRequest("Boleto já foi depositado.");
        //}

        //if(billet.DateExpiration < DateTime.Now)
        //{
        //    return BadRequest("Boleto expirado, tente novamente mais tarde.");
        //}

        bus.Publish(new NewDepositAccountProducer(billet.AccountId, billet.DepositId, billet.AccountNumber, billet.Amount));

        return Ok("Depositamos seu boleto. Entre 1 a 30 min seu dinheira cairá na sua conta.");
    }
}
