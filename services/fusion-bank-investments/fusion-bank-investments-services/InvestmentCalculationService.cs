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
            var currentDate = DateTime.UtcNow;

            // Busca a taxa Selic diária para CDB, LCI ou LCA
            decimal dailySelicRate = 0;
            if (investment.InvestmentType is InvestmentType.CDB or InvestmentType.LCI or InvestmentType.LCA)
            {
                var selicRate = await _investmentService.GetSelicRateAsync();
                dailySelicRate = selicRate;
            }

            foreach (var entry in investment.Entries.Where(d => decimal.IsPositive(d.Amount)))
            {
                // Calcular dias úteis (substituir por uma função real de dias úteis)
                var days = CalculateBusinessDays(entry.Date, currentDate);
                if (days < 0)
                {
                    continue;
                }

                switch (investment.InvestmentType)
                {
                    case InvestmentType.CDB:
                        // Rendimento: 108% da Selic diária
                        var cdbDailyRate = dailySelicRate * 1.08m;
                        var grossReturn = entry.Amount * (decimal)Math.Pow(1 + (double)cdbDailyRate, days) - entry.Amount;
                        // Aplicar imposto de renda
                        var taxRate = days <= 180 ? 0.225m : (days <= 360 ? 0.2m : (days <= 720 ? 0.175m : 0.15m));
                        totalPaidOff += grossReturn * (1 - taxRate);
                        break;

                    case InvestmentType.LCI:
                    case InvestmentType.LCA:
                        // Rendimento: 95% da Selic diária
                        var lciLcaDailyRate = dailySelicRate * 0.95m;
                        totalPaidOff += entry.Amount * (decimal)Math.Pow(1 + (double)lciLcaDailyRate, days) - entry.Amount;
                        break;

                    case InvestmentType.STOCK:
                    case InvestmentType.FII:
                        if (entry.Quantity > 0 && entry.UnitPrice >= 0)
                        {
                            totalPaidOff += (investment.CurrentMarketValue - entry.UnitPrice) * entry.Quantity;
                        }
                        break;

                    default:
                        break;
                }
            }

            investment.PaidOff = totalPaidOff;
        }

        // Função auxiliar para calcular dias úteis (implementação simplificada)
        private double CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            // Implementação simplificada: usar TotalDays e ajustar para 252 dias úteis por ano
            // Para precisão, use uma biblioteca como Nager.Date para excluir feriados
            var totalDays = (endDate - startDate).TotalDays;
            if (totalDays <= 0) return 0;
            // Aproximação: 252 dias úteis em 365 dias corridos
            return totalDays * (252.0 / 365.0);
        }
    }
}
