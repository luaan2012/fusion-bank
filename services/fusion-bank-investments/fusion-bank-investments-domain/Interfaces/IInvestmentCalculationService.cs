using fusion.bank.investments.domain.Models;

namespace fusion.bank.investments.domain.Interfaces
{
    public interface IInvestmentCalculationService
    {
        Task CalculatePaidOffAsync(Investment investment);
        Task CalculateMarketValueAsync(Investment investment);
        void CalculateTotalBalance(Investment investment);
        void UpdateBalance(Investment investment);
    }
}
