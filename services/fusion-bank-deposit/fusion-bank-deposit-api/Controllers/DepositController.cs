using fusion.bank.core;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.core.Messages.Requests;
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
    
    [HttpPost("deposit-billet")]
    public async Task<IActionResult> Depositbillet(DepositRequest depositRequest)
    {
        var deposit = await depositRepository.GetDepositByCode(depositRequest.Code);

        if (deposit is null)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Nenhum boleto foi encontrado.");
        }

        if (deposit.Debited)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Boleto já foi pago.");

        }

        await bus.Publish(new TransactionRequest(deposit.AccountId, deposit.DepositId, deposit.AccountNumber, deposit.AgencyNumber, deposit.Amount, deposit.BilletType, depositRequest.PaymentType));

        await bus.Publish(GenerateEvent.CreateDepositCreateEvent(deposit.AccountId.ToString(), deposit.Amount, TransferType.BOLETO));

        return CreateResponse(new DataContractMessage<Deposit>() { Success = true }, "Boleto pago com sucesso.");
    }


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


    [HttpPost("direct-deposit")]
    public async Task<IActionResult> DirectDeposit(DirectDeposit directDeposit)
    {

        if (directDeposit.Amount <= 0)
        {
            return CreateResponse(new DataContractMessage<Deposit>() { Success = false }, "Valor inválido.");
        }

        var deposit = new Deposit();

        deposit.GenerateDepositDirect(directDeposit);

        await bus.Publish(new NewDepositAccountProducer(deposit.AccountId, deposit.DepositId, deposit.AccountNumber, deposit.AgencyNumber, deposit.Amount, deposit.Description, deposit.BilletType));

        await bus.Publish(GenerateEvent.CreateDepositCreateEvent(deposit.AccountId.ToString(), deposit.Amount, TransferType.BOLETO));

        return CreateResponse(new DataContractMessage<Deposit>() { Success = true }, "Deposito recebido! Entre 5 a 10 min seu dinheira cairá na sua conta.");
    }
}
