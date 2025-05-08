using fusion.bank.core;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.deposit.domain.Interfaces;
using fusion.bank.deposit.domain.Models;
using fusion.bank.deposit.domain.Requests;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion_bank_deposit_api.Controllers;

[ApiController]
[Route("[controller]")]
public class DepositController(IDepositRepository depositRepository, 
    ILogger<DepositController> logger, IPublishEndpoint bus) : MainController
{
   
    [HttpPost("generate-billet")]
    public async Task<IActionResult> GenerateBillet(BilletRequest billetRequest)
    {
        var deposit = new Deposit();

        deposit.GenerateCode(billetRequest);

        await depositRepository.SaveDeposit(deposit);

        await bus.Publish(GenerateEvent.CreateBilletGeneratedEvent(billetRequest.AccountId.ToString(), deposit.CodeGenerate));

        return CreateResponse(new DataContractMessage<Deposit>() { Data = deposit, Success = true});
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
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Nenhum boleto foi encontrado.");
        }

        if (billet.Debited)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Boleto já foi depositado.");

        }

        if (billet.DateExpiration < DateTime.Now)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Boleto expirado, tente novamente mais tarde.");
        }

        await bus.Publish(new NewDepositAccountProducer(billet.AccountId, billet.DepositId, billet.AccountNumber, billet.Amount));

        await bus.Publish(GenerateEvent.CreateDepositEvent(billet.AccountId.ToString(), billet.Amount, TransferType.BOLETO));

        return CreateResponse(new DataContractMessage<Deposit>() { Success = true }, "Depositamos seu boleto. Entre 1 a 30 min seu dinheira cairá na sua conta.");
    }
}
