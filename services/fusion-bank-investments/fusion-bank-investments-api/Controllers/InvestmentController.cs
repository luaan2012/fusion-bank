using fusion.bank.core;
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
        public async Task<IActionResult> Invest(InvestmentRequest investment)
        {
            if (investment is null)
                return CreateResponse(new DataContractMessage<string>(), "Nenhuma solicitação de investimento fornecida.");

            decimal totalAmount = investment.Amount;
            var availableInvestments = await _investmentService.GetAvailableInvestmentsAsync();

            var investmentSelected = availableInvestments.FirstOrDefault(d => d.Symbol == investment.Symbol);

            if (investmentSelected is null)
            {
                return CreateResponse(new DataContractMessage<string>(), $"Investimento inválido: {investment.Symbol} não encontrado ou tipo incorreto.");
            }

            var accountInformation = await _requestClient.GetResponse<DataContractMessage<AccountInformationResponse>>(
                new NewAccountRequestInformation(investment.AccountId));

            if (!accountInformation.Message.Success)
                return CreateResponse(new DataContractMessage<string>(), "Não foi possível prosseguir com sua solicitação, tente novamente mais tarde.");

            if (totalAmount > accountInformation.Message.Data.Balance)
                return CreateResponse(new DataContractMessage<string>(), "A conta não possui saldo suficiente para essa transação.");

            
            var requestDebitAccount = await _requestInvestment.GetResponse<DataContractMessage<AccountInvestmentResponse>>(
                new NewInvestmentRequest(investment.AccountId, investment.Amount));

            if (!requestDebitAccount.Message.Success)
                return CreateResponse(new DataContractMessage<string>(), "Não foi possível prosseguir com sua solicitação, tente novamente mais tarde.");

            await HandleInvestment(investment, investmentSelected);

            await _publishEndpoint.Publish(GenerateEvent.CreateInvestmentEvent(
                investment.AccountId.ToString(),
                investment.Symbol,
                investment.Amount,
                investment.InvestmentType));
          
            return CreateResponse(new DataContractMessage<string> { Success = true }, "Parabéns! Você deu o primeiro passo para se tornar milionário.");
        }

        [HttpPut("handle-investment")]
        public async Task<IActionResult> HandleInvestment(Guid accountId, Guid investmentId, decimal amount)
        {
            var investment = await _investmentRepository.GetInvestmentById(investmentId, accountId);

            if (investment is null)
                return CreateResponse(new DataContractMessage<List<Investment>> { Success = false }, "Nenhum investimento foi encontrado.");
            
            investment.RegularMarketPrice = await _investmentService.GetCurrentPriceAsync(investment.Symbol);

            var accountRequest = await _requestInvestment.GetResponse<DataContractMessage<AccountInvestmentResponse>>(
                    new NewInvestmentRequest(investment.AccountId, amount));

            if (!accountRequest?.Message?.Success ?? false)
            {
                return CreateResponse(new DataContractMessage<List<Investment>> { Success = false }, "Aconteceu um erro ao tentar resgatar seu dinheiro, mas fique tranquilo... já estamos processando sua solicitação.");
            }

            decimal quantity = investment.RegularMarketPrice > 0 ? amount / investment.RegularMarketPrice : 0;
            investment.AddInvestment(amount, DateTime.Now, quantity, investment.RegularMarketPrice);
            await _calculationService.CalculatePaidOffAsync(investment);

            await _investmentRepository.Update(investment);

            await _publishEndpoint.Publish(GenerateEvent.CreateInvestmentPurchaseEvent(
                investment.AccountId.ToString(),
                investment.Symbol,
                amount,
                investment.InvestmentType));

            return CreateResponse(new DataContractMessage<List<Investment>> { Success = true }, "Já estamos processando sua solicitação, aguarde alguns instantes");
        }

        [HttpPut("rescue-investment/{accountId}")]
        public async Task<IActionResult> RescueInvestment(Guid accountId, Guid investmentId, decimal amount)
        {
            var investment = await _investmentRepository.GetInvestmentById(investmentId, accountId);

            await _calculationService.CalculatePaidOffAsync(investment);

            if (investment is null)
                return CreateResponse(new DataContractMessage<List<Investment>> { Success = false }, "Nenhum investimento foi encontrado.");


            if (investment.TotalBalance < Math.Abs(amount))
                return CreateResponse(new DataContractMessage<string> { Success = false }, "Valor solicitado é maior que o investido.");

            var accountRequest = await _requestInvestmentPut.GetResponse<DataContractMessage<AccountInvestmentResponse>>(
                    new NewAccountRequestPutAmount(investment.AccountId, amount));

            if (!accountRequest?.Message?.Success ?? false)
            {
                return CreateResponse(new DataContractMessage<List<Investment>> { Success = false }, "Aconteceu um erro ao tentar resgatar seu dinheiro, mas fique tranquilo... já estamos processando sua solicitação.");
            }

            decimal quantity = investment.RegularMarketPrice > 0 ? amount / investment.RegularMarketPrice : 0;

            investment.RescueInvestment(amount, DateTime.Now, quantity, investment.RegularMarketPrice);

            await _investmentRepository.Update(investment);

            await _publishEndpoint.Publish(GenerateEvent.CreateInvestmentRescueEvent(
                investment.AccountId.ToString(),
                investment.Symbol,
                amount,
                investment.InvestmentType));

            return CreateResponse(new DataContractMessage<List<Investment>> { Success = true }, "Já estamos processando sua solicitação, aguarde alguns instantes");
        }

        [HttpGet("list-all-investments")]
        public async Task<IActionResult> GetAllInvestments()
        {
            var investments = await _investmentRepository.GetAllInvestment();

            foreach (var investment in investments)
            {
                await _calculationService.CalculatePaidOffAsync(investment);
                //investment.CalculateTotalBalance();
            }

            return CreateResponse(new DataContractMessage<List<Investment>> { Data = investments, Success = true });
        }

        [HttpGet("list-investments/{accountId}")]
        public async Task<IActionResult> ListInvestmentsId(Guid accountId, int limit)
        {
            var investments = await _investmentRepository.ListInvestmentByAccountId(accountId, limit);

            foreach (var investment in investments)
            {
                await _calculationService.CalculatePaidOffAsync(investment);
                //investment.CalculateTotalBalance();
            }

            return CreateResponse(new DataContractMessage<List<Investment>> { Data = investments, Success = true });
        }

        [HttpGet("available-investments")]
        public async Task<IActionResult> GetAvailableInvestments(string accountId)
        {
            var investments = await _investmentService.GetAvailableInvestmentsAsync();
            var investmentsUser = Guid.TryParse(accountId, out Guid accountIdGuid) ? await _investmentRepository.ListInvestmentByAccountId(accountIdGuid) : null;

            if (investmentsUser != null)
            {
                var userInvestmentsBySymbol = investmentsUser.ToDictionary(
                    e => e.Symbol,
                    e => e.Id,
                    StringComparer.OrdinalIgnoreCase // Optional: case-insensitive Symbol comparison
                );

                investments = investments.Select(d =>
                {
                    if (userInvestmentsBySymbol.TryGetValue(d.Symbol, out Guid userInvestmentId))
                    {
                        d.Id = userInvestmentId;
                        d.OnMyPocket = true;
                    }
                    else
                    {
                        d.Id = Guid.Empty; // Or keep existing Id, depending on requirements
                        d.OnMyPocket = false;
                    }
                    return d;

                }).ToList();
            }

            return CreateResponse(new DataContractMessage<List<AvailableInvestment>>
            {
                Data = investments,
                Success = true
            });
        }

        private async Task HandleInvestment(InvestmentRequest investmentRequest, AvailableInvestment availableInvestment)
        {
            decimal unitPrice = availableInvestment.RegularMarketPrice;

            decimal quantity = unitPrice > 0 ? investmentRequest.Amount / unitPrice : 0;

            var investment = new Investment()
            {
                AccountId = investmentRequest.AccountId,
                DateInvestment = DateTime.Now,
                Id = Guid.NewGuid(),
                InvestmentType = investmentRequest.InvestmentType,
                UnitPrice = unitPrice,
                Symbol = investmentRequest.Symbol,
                RegularMarketPrice = availableInvestment.RegularMarketPrice,
                Logourl = availableInvestment.Logourl,
                ShortName = availableInvestment.ShortName,
                Name = availableInvestment.LongName,
            };

            investment.AddInvestment(investmentRequest.Amount, DateTime.Now, quantity, unitPrice);
            await _investmentRepository.SaveInvestment(investment);
        }
    }
}