using fusion.bank.core.Enum;
using fusion.bank.core.Model;
using fusion.bank.core.Request;

namespace fusion.bank.account.domain.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveAccount(Account account);
        Task<IEnumerable<Account>> ListAllAccount();
        Task<Account> ListAccountById(Guid id);
        Task<Account> ListAccountByNumberAccount(string accountNumber);
        Task<Account> GetAccountPerTypeAndPassoword(LoginRequest loginRequest);
        Task UpdateAccount(Account account);
        Task<Account> ListAccountByKey(string keyAccount);
        Task SaveKeyByAccount(Guid idAccount, string keyAccount);
    }
}
