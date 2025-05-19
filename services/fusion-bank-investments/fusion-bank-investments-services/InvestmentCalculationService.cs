using fusion.bank.core.Enum;
using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.domain.Models;

namespace fusion.bank.investments.Services
{
    public class InvestmentCalculationService : IInvestmentCalculationService
    {
        private readonly IInvestmentService _investmentService;

        public InvestmentCalculationService(IInvestmentService investmentService)
        {
            _investmentService = investmentService ?? throw new ArgumentNullException(nameof(investmentService));
        }

        public async Task CalculatePaidOffAsync(Investment investment)
        {
            decimal totalPaidOff = 0;
            var selicRate = await _investmentService.GetSelicRateAsync();
            var dailyRate = selicRate / 365;

            foreach (var entry in investment.Entries.Where(d => decimal.IsPositive(d.Amount)))
            {
                var days = (DateTime.Now - entry.Date).TotalDays;
                switch (investment.InvestmentType)
                {
                    case InvestmentType.CDB:
                        totalPaidOff += entry.Amount * (decimal)Math.Pow(1 + (double)((double)dailyRate * 1.08), days) - entry.Amount;
                        break;
                    case InvestmentType.LCI:
                    case InvestmentType.LCA:
                        totalPaidOff += entry.Amount * (decimal)Math.Pow(1 + (double)((double)dailyRate * 0.95), days) - entry.Amount;
                        break;
                    case InvestmentType.Stock:
                    case InvestmentType.FII:
                        break;
                    default:
                        totalPaidOff += 0;
                        break;
                }
            }

            investment.PaidOff = totalPaidOff;
        }

        public async Task CalculateMarketValueAsync(Investment investment)
        {
            if (investment.InvestmentType == InvestmentType.Stock || investment.InvestmentType == InvestmentType.FII)
            {
                var currentPrice = await _investmentService.GetCurrentPriceAsync(investment.Symbol);
                investment.CurrentMarketValue = investment.Quantity * currentPrice;
            }

            investment.CurrentMarketValue = 0;
        }

        public void CalculateTotalBalance(Investment investment)
        {
            if (investment.InvestmentType == InvestmentType.Stock || investment.InvestmentType == InvestmentType.FII)
            {
                investment.Balance = investment.CurrentMarketValue;
            }
            decimal totalInvested = investment.Entries.Sum(entry => entry.Amount);
            investment.Balance = totalInvested + investment.PaidOff;
        }

        public void UpdateBalance(Investment investment)
        {
            investment.Balance = investment.Entries.Sum(d => d.Amount);
        }
    }
}
