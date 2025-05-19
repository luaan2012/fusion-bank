using fusion.bank.investments.domain.Models;

namespace fusion.bank.investments.domain.Interfaces
{
    public interface IInvestmentService
    {
        Task<decimal> GetSelicRateAsync();
        Task<decimal> GetCurrentPriceAsync(string symbol);
        Task<List<AvailableInvestment>> GetAvailableInvestmentsAsync();
    }
}
