using fusion.bank.core;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.deposit.domain.Interfaces;
using fusion.bank.deposit.domain.Models;
using fusion.bank.deposit.domain.Requests;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.deposit.api.Controllers;

[ApiController]
[Route("[controller]")]
public class DepositController(IDepositRepository depositRepository, 
    ILogger<DepositController> logger, IPublishEndpoint bus) : MainController
{
   
    [HttpPost("generate-billet")]
    public async Task<IActionResult> GenerateBillet(DepositBilletRequest billetRequest)
    {
        var deposit = new Deposit();

        deposit.GenerateCode(billetRequest);

        await depositRepository.SaveDeposit(deposit);

        await bus.Publish(GenerateEvent.CreateBilletGeneratedEvent(billetRequest.AccountId.ToString(), deposit.CodeGenerate));

        return CreateResponse(new DataContractMessage<string>() { Data = deposit.CodeGenerate, Success = true});
    }

    [HttpGet("get-billet-by-id/{billetCode}")]
    public async Task<IActionResult> GenerateBillet(string billetCode)
    {
        var deposit = await depositRepository.GetDepositByCode(billetCode);

        if (deposit is null)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Boleto nao existe ou nao foi registrado ainda.");
        }

        return CreateResponse(new DataContractMessage<Deposit>() { Data = deposit, Success = true });
    }


    [HttpGet("list-all-billets")]
    public async Task<IActionResult> ListAllBillets()
    {
        return Ok(await depositRepository.ListAllBillets());
    }

    [HttpGet("deposit-billet/{codeBillet}")]
    public async Task<IActionResult> Depositbillet(string codeBillet)
    {
        var deposit = await depositRepository.GetDepositByCode(codeBillet);

        if(deposit is null)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Nenhum boleto foi encontrado.");
        }

        if (deposit.Debited)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Boleto já foi pago.");

        }

        if (deposit.DateExpiration < DateTime.Now)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Boleto expirado, tente novamente mais tarde.");
        }

        await bus.Publish(new NewDepositAccountProducer(deposit.AccountId, deposit.DepositId, deposit.AccountNumber, deposit.AgencyNumber, deposit.Amount, deposit.Description));

        await bus.Publish(GenerateEvent.CreateDepositEvent(deposit.AccountId.ToString(), deposit.Amount, TransferType.BOLETO));

        return CreateResponse(new DataContractMessage<Deposit>() { Success = true }, "Depositamos seu boleto. Entre 1 a 30 min seu dinheira cairá na sua conta.");
    }

    [HttpPost("direct-deposit")]
    public async Task<IActionResult> DirectDeposit(DirectDeposit directDeposit)
    {

        if (directDeposit.Amount <= 0)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Valor inválido.");
        }

        var deposit = new Deposit();

        deposit.GenerateDepositDirect(directDeposit);

        await bus.Publish(new NewDepositAccountProducer(deposit.AccountId, deposit.DepositId, deposit.AccountNumber, deposit.AgencyNumber, deposit.Amount, deposit.Description));

        await bus.Publish(GenerateEvent.CreateDepositEvent(deposit.AccountId.ToString(), deposit.Amount, TransferType.BOLETO));

        return CreateResponse(new DataContractMessage<Deposit>() { Success = true }, "Deposito recebido! Entre 5 a 10 min seu dinheira cairá na sua conta.");
    }
}
