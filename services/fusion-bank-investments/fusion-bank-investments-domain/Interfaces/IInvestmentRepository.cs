using fusion.bank.investments.domain.Models;

namespace fusion.bank.investments.domain.Interfaces
{
    public interface IInvestmentRepository
    {
        Task<Investment> GetInvestmentById(Guid guid, Guid accountId);
        Task<List<Investment>> ListInvestmentByAccountId(Guid accountId, int limit = int.MaxValue);
        Task<List<Investment>> GetAllInvestment();
        Task DeleteInvestmentById(Guid guid);
        Task Update(Investment investment);
        Task SaveInvestment(Investment investment);
    }
}
