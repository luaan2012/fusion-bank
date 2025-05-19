using fusion.bank.investments.domain.Models;

namespace fusion.bank.investments.domain.Interfaces
{
    public interface IInvestmentService
    {
        Task<decimal> GetSelicRateAsync();
        Task UpdateInvestmentAsync(AvailableInvestment investment);
        Task<List<AvailableInvestment>> GetAvailableInvestmentsAsync();
        Task<decimal> GetCurrentPriceAsync(string symbol);
    }
}
