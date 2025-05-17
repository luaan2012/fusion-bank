using fusion.bank.account.domain.Request;
using fusion.bank.core.Model;

namespace fusion.bank.account.domain.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveAccount(Account account);
        Task<IEnumerable<Account>> ListAllAccount();
        Task<Account> ListAccountById(Guid id);
        Task<Account> ListAccountByNumberAgencyAccount(string accountNumber, string agencyNumber);
        Task<Account> ListAccountByNumberAccount(string accountNumber);
        Task<Account> GetAccountPerTypeAndPassoword(LoginRequest loginRequest);
        Task<Account> EditAccount(Guid accountId, AccountEditRequest accountEditRequest);
        Task<Account> DeleteKeyAccount(Guid accountId);
        Task<Account> EditKeyAccount(Guid accountId, string keyAccount);
        Task UpdateAccount(Account account);
        Task<bool> DeleteAccount(Guid accountId);
        Task<Account> ListAccountByKey(string keyAccount);
        Task SaveKeyByAccount(RegisterKeyRequest registerKey);
        Task SetDarkMode(Guid idAccount, bool darkMode);
        Task RegisterPasswordTransaction(Guid accountId, string passwordTransaction);
    }
}
