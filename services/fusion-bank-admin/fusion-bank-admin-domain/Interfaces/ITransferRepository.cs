using fusion.bank.admin.domain.Models;

namespace fusion.bank.admin.domain.Interfaces
{
    public interface ITransferRepository
    {
        Task<List<TransferSummary>> GetTransferSummaryAsync(string period);

    }
}
