using fusion.bank.admin.domain.Models;

namespace fusion.bank.admin.domain.Interfaces
{
    public interface IDepositRepository
    {
        Task<BilletsSummary> GetBoletoSummaryAsync();
        Task<DepositSummary> GetDepositSummaryAsync();
    }
}
