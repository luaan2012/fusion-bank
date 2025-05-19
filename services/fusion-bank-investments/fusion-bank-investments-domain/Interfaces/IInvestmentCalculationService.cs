using fusion.bank.investments.domain.Models;

namespace fusion.bank.investments.domain.Interfaces
{
    public interface IInvestmentCalculationService
    {
        Task CalculatePaidOffAsync(Investment investment);
    }
}
