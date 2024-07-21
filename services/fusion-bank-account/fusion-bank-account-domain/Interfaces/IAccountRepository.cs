using fusion.bank.core.Model;

namespace fusion.bank.account.domain.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveAccount(Account account);
        Task<IEnumerable<Account>> ListAllAccount();
        Task<Account> ListAccountById(Guid id);
        Task<Account> ListAccountByNumberAccount(string accountNumber);
        Task UpdateAccount(Account account);
        Task<Account> ListAccountByKey(string keyAccount);
        Task SaveKeyByAccount(Guid idAccount, string keyAccount);
    }
}
