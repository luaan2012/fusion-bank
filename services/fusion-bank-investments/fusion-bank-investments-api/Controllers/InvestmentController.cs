using fusion.bank.core;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.domain.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion_bank_investments_api.Controllers;

[ApiController]
[Route("[controller]")]
public class InvestmentController(IRequestClient<NewAccountRequestInformation> requestClient, 
    IRequestClient<NewInvestmentRequest> requestInvestment,
    IRequestClient<NewAccountRequestPutAmount> requestInvestmentPut,
    IPublishEndpoint publishEndpoint,
    IInvestmentRepository investmentRepository) : MainController
{
    [HttpPost("create-invest")]
    public async Task<IActionResult> Invest(InvestmentRequest investment)
    {
        var accountInformation = await requestClient.GetResponse<DataContractMessage<AccountInformationResponse>>(new NewAccountRequestInformation(investment.AccountId));

        if (!accountInformation.Message.Success)
        {
            return CreateResponse(new DataContractMessage<string>(), $"Nao foi possivel prosseguir com sua solicitacao, tente novamente mais tarde.");
        }

        if(investment.Amount > accountInformation.Message.Data.Balance)
        {
            return CreateResponse(new DataContractMessage<string>(), $"A Conta nao possui saldo suficiente para essa transacao.");
        }

        var requestDebitAccount = await requestInvestment.GetResponse<DataContractMessage<AccountInvestmentResponse>>(new NewInvestmentRequest(investment.AccountId, investment.Amount));

        if (!requestDebitAccount.Message.Success)
        {
            return CreateResponse(new DataContractMessage<string>(), $"Nao foi possivel prosseguir com sua solicitacao, tente novamente mais tarde.");
        }

        await HandleInvestment(investment);

        await publishEndpoint.Publish(GenerateEvent.CreateInvestmentEvent(investment.AccountId.ToString(), investment.Amount, investment.InvestmenType.ToString()));

        return CreateResponse(new DataContractMessage<string>() { Success = true }, $"Parabens! Voce deu o primeiro passo para se tornar milionario.");
    }

    [HttpPost("handle-investment")]
    public async Task<IActionResult> HandleInvestment(Guid accountId, decimal amount)
    {
        var investment = await investmentRepository.GetInvestmentByAccountId(accountId);

        if(investment is null) return CreateResponse(new DataContractMessage<List<Investment>> { Success = false}, "Nenhum investimento foi encontrado.");

        if (decimal.IsNegative(amount) && investment.TotalBalance < Math.Abs(amount)) return CreateResponse(new DataContractMessage<string> { Success = false }, "Valor solicitado é maior que o investido.");

        Response<DataContractMessage<AccountInvestmentResponse>> accountRequest = null;

        if (decimal.IsPositive(amount))
        {
            accountRequest = await requestInvestment.GetResponse<DataContractMessage<AccountInvestmentResponse>>(new NewInvestmentRequest(investment.AccountId, amount));
        }

        if(decimal.IsNegative(amount))
        {
            accountRequest = await requestInvestmentPut.GetResponse<DataContractMessage<AccountInvestmentResponse>>(new NewAccountRequestPutAmount(investment.AccountId, Math.Abs(amount)));
        }

        if (!accountRequest?.Message?.Success ?? false)
        {
            return CreateResponse(new DataContractMessage<List<Investment>> { Success = false }, "Aconteceu um erro ao tentar resgatar seu dinheiro, mas fique tranquilo... já estamos processando sua solicitacao.");
        }

        if (investment.TotalBalance == amount)
        {
            await investmentRepository.DeleteInvestmentById(investment.Id);
        }

        if (investment.TotalBalance != amount)
        {
            investment.AddInvestment(amount, DateTime.Now);
            investment.UpdateBalance();

            await investmentRepository.Update(investment);
        }

        return CreateResponse(new DataContractMessage<List<Investment>> { Success = true }, "Já estamos processando sua solicitacao, aguarde alguns instantes");
    }

    [HttpPost("get-all-investment")]
    public async Task<IActionResult> GetAllInvestments()
    {
        var investment = await investmentRepository.GetAllInvestment();

        return CreateResponse(new DataContractMessage<List<Investment>> { Data = investment, Success = true });
    }

    private async Task HandleInvestment(InvestmentRequest investmentRequest)
    {
        var investment = new Investment
        {
            AccountId = investmentRequest.AccountId,
            Balance = investmentRequest.Amount,
            DateInvestment = DateTime.Now,
            Id = Guid.NewGuid(),
            InvestmentType = investmentRequest.InvestmenType
        };

        investment.AddInvestment(investmentRequest.Amount, DateTime.Now);

        await investmentRepository.SaveInvestment(investment);
    }
}
