using fusion.bank.core;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.domain.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion_bank_investments_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvestmentController : MainController
    {
        private readonly IRequestClient<NewAccountRequestInformation> _requestClient;
        private readonly IRequestClient<NewInvestmentRequest> _requestInvestment;
        private readonly IRequestClient<NewAccountRequestPutAmount> _requestInvestmentPut;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestmentService _investmentService;
        private readonly IInvestmentCalculationService _calculationService;

        public InvestmentController(
            IRequestClient<NewAccountRequestInformation> requestClient,
            IRequestClient<NewInvestmentRequest> requestInvestment,
            IRequestClient<NewAccountRequestPutAmount> requestInvestmentPut,
            IPublishEndpoint publishEndpoint,
            IInvestmentRepository investmentRepository,
            IInvestmentService investmentService,
            IInvestmentCalculationService calculationService)
        {
            _requestClient = requestClient;
            _requestInvestment = requestInvestment;
            _requestInvestmentPut = requestInvestmentPut;
            _publishEndpoint = publishEndpoint;
            _investmentRepository = investmentRepository;
            _investmentService = investmentService;
            _calculationService = calculationService;
        }

        [HttpPost("create-invest")]
        public async Task<IActionResult> Invest([FromBody] List<InvestmentRequest> investments)
        {
            if (!investments.Any())
                return CreateResponse(new DataContractMessage<string>(), "Nenhuma solicitação de investimento fornecida.");

            Guid accountId = investments.First().AccountId;
            if (investments.Exists(i => i.AccountId != accountId))
                return CreateResponse(new DataContractMessage<string>(), "Todos os investimentos devem pertencer à mesma conta.");

            decimal totalAmount = investments.Sum(i => i.Amount);
            var availableInvestments = await _investmentService.GetAvailableInvestmentsAsync();

            foreach (var investment in investments)
            {
                if (!availableInvestments.Exists(ai => ai.Symbol == investment.Symbol && ai.Type == investment.InvestmenType))
                    return CreateResponse(new DataContractMessage<string>(), $"Investimento inválido: {investment.Symbol} não encontrado ou tipo incorreto.");
            }

            var accountInformation = await _requestClient.GetResponse<DataContractMessage<AccountInformationResponse>>(
                new NewAccountRequestInformation(accountId));

            if (!accountInformation.Message.Success)
                return CreateResponse(new DataContractMessage<string>(), "Não foi possível prosseguir com sua solicitação, tente novamente mais tarde.");

            if (totalAmount > accountInformation.Message.Data.Balance)
                return CreateResponse(new DataContractMessage<string>(), "A conta não possui saldo suficiente para essa transação.");

            foreach (var investment in investments)
            {
                var requestDebitAccount = await _requestInvestment.GetResponse<DataContractMessage<AccountInvestmentResponse>>(
                    new NewInvestmentRequest(investment.AccountId, investment.Amount));

                if (!requestDebitAccount.Message.Success)
                    return CreateResponse(new DataContractMessage<string>(), "Não foi possível prosseguir com sua solicitação, tente novamente mais tarde.");

                await HandleInvestment(investment);

                await _publishEndpoint.Publish(GenerateEvent.CreateInvestmentEvent(
                    investment.AccountId.ToString(),
                    investment.Amount,
                    investment.Symbol));
            }

            return CreateResponse(new DataContractMessage<string> { Success = true }, "Parabéns! Você deu o primeiro passo para se tornar milionário.");
        }

        [HttpGet("handle-investment")]
        public async Task<IActionResult> HandleInvestment(Guid accountId, Guid investmentId, decimal amount)
        {
            var investment = await _investmentRepository.GetInvestmentById(investmentId, accountId);

            if (investment is null)
                return CreateResponse(new DataContractMessage<List<Investment>> { Success = false }, "Nenhum investimento foi encontrado.");

            _calculationService.UpdateBalance(investment);
            _calculationService.CalculateTotalBalance(investment);
            await _calculationService.CalculatePaidOffAsync(investment);

            if (decimal.IsNegative(amount) && investment.TotalBalance < Math.Abs(amount))
                return CreateResponse(new DataContractMessage<string> { Success = false }, "Valor solicitado é maior que o investido.");

            Response<DataContractMessage<AccountInvestmentResponse>> accountRequest = null;

            if (decimal.IsPositive(amount))
            {
                accountRequest = await _requestInvestment.GetResponse<DataContractMessage<AccountInvestmentResponse>>(
                    new NewInvestmentRequest(investment.AccountId, amount));
            }
            else if (decimal.IsNegative(amount))
            {
                accountRequest = await _requestInvestmentPut.GetResponse<DataContractMessage<AccountInvestmentResponse>>(
                    new NewAccountRequestPutAmount(investment.AccountId, Math.Abs(amount)));
            }

            if (!accountRequest?.Message?.Success ?? false)
            {
                return CreateResponse(new DataContractMessage<List<Investment>> { Success = false }, "Aconteceu um erro ao tentar resgatar seu dinheiro, mas fique tranquilo... já estamos processando sua solicitação.");
            }

            if (decimal.IsNegative(amount) && investment.TotalBalance == Math.Abs(amount))
            {
                await _investmentRepository.DeleteInvestmentById(investment.Id);
            }
            else
            {
                decimal unitPrice = investment.InvestmentType == InvestmentType.Stock || investment.InvestmentType == InvestmentType.FII
                    ? await _investmentService.GetCurrentPriceAsync(investment.Symbol)
                    : 0;
                decimal quantity = unitPrice > 0 ? amount / unitPrice : 0;
                investment.AddInvestment(amount, DateTime.Now, quantity, unitPrice);
                _calculationService.UpdateBalance(investment);

                await _investmentRepository.Update(investment);
            }

            return CreateResponse(new DataContractMessage<List<Investment>> { Success = true }, "Já estamos processando sua solicitação, aguarde alguns instantes");
        }

        [HttpGet("list-all-investments")]
        public async Task<IActionResult> GetAllInvestments()
        {
            var investments = await _investmentRepository.GetAllInvestment();
            investments.ForEach(async d =>
            {
                _calculationService.UpdateBalance(d);
                _calculationService.CalculateTotalBalance(d);
                await _calculationService.CalculatePaidOffAsync(d);
            });
            return CreateResponse(new DataContractMessage<List<Investment>> { Data = investments, Success = true });
        }

        [HttpGet("list-investments/{accountId}")]
        public async Task<IActionResult> ListInvestmentsId(Guid accountId, int limit)
        {
            var investments = await _investmentRepository.ListInvestmentByAccountId(accountId, limit);
            investments.ForEach(async d =>
            {
                _calculationService.UpdateBalance(d);
                _calculationService.CalculateTotalBalance(d);
                await _calculationService.CalculatePaidOffAsync(d);
            });
            return CreateResponse(new DataContractMessage<List<Investment>> { Data = investments, Success = true });
        }

        [HttpGet("available-investments")]
        public async Task<IActionResult> GetAvailableInvestments()
        {
            var investments = await _investmentService.GetAvailableInvestmentsAsync();

            return CreateResponse(new DataContractMessage<List<AvailableInvestment>>
            {
                Data = investments,
                Success = true
            });
        }

        private async Task HandleInvestment(InvestmentRequest investmentRequest)
        {
            decimal unitPrice = investmentRequest.InvestmenType == InvestmentType.Stock || investmentRequest.InvestmenType == InvestmentType.FII
                ? await _investmentService.GetCurrentPriceAsync(investmentRequest.Symbol)
                : 0;
            decimal quantity = unitPrice > 0 ? investmentRequest.Amount / unitPrice : 0;

            var investment = new Investment()
            {
                AccountId = investmentRequest.AccountId,
                Balance = investmentRequest.Amount,
                DateInvestment = DateTime.Now,
                Id = Guid.NewGuid(),
                InvestmentType = investmentRequest.InvestmenType,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Symbol = investmentRequest.Symbol
            };

            investment.AddInvestment(investmentRequest.Amount, DateTime.Now, quantity, unitPrice);
            await _investmentRepository.SaveInvestment(investment);
        }
    }
}