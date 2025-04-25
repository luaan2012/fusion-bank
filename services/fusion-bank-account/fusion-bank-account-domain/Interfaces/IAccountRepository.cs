using fusion.bank.account.domain.Request;
using fusion.bank.core.Model;

namespace fusion.bank.account.domain.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveAccount(Account account);
        Task<IEnumerable<Account>> ListAllAccount();
        Task<Account> ListAccountById(Guid id);
        Task<Account> ListAccountByNumberAccount(string accountNumber);
        Task<Account> GetAccountPerTypeAndPassoword(LoginRequest loginRequest);
        Task<Account> EditAccountByKey(string keyAccount, AccountEditRequest accountEditRequest);
        Task<Account> DeleteKeyAccount(Guid accountId);
        Task<Account> EditKeyAccount(Guid accountId, string keyAccount);
        Task UpdateAccount(Account account);
        Task<bool> DeleteAccount(Guid accountId);
        Task<Account> ListAccountByKey(Guid accountId, string keyAccount);
        Task SaveKeyByAccount(Guid idAccount, string keyAccount);
        Task SetDarkMode(Guid idAccount, bool darkMode);
    }
}
