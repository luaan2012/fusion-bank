using fusion.bank.core.Model;

namespace fusion.bank.account.domain.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveAccount(Account account);
        Task<IEnumerable<Account>> ListAllAccount();
        Task<Account> ListAccountById(Guid id);
        Task UpdateAccount(Account account);
    }
}
