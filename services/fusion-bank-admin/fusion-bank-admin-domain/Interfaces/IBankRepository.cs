using fusion.bank.admin.domain.Models;

namespace fusion.bank.admin.domain.Interfaces
{
    public interface IBankRepository
    {
        Task<BankSummary> GetBankSummaryAsync();
        Task<List<BankListItem>> GetBankListAsync();
        Task<List<BankAccountItem>> GetBankAccountsAsync(string bankCode);
        Task<bool> UpdateBankAsync(string bankCode, BankUpdateModel updateModel);
    }
}
