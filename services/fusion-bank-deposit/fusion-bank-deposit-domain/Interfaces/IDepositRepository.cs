using fusion.bank.deposit.domain.Models;

namespace fusion.bank.deposit.domain.Interfaces
{
    public interface IDepositRepository
    {
        Task<Deposit> GetDepositById(Guid id);
        Task<Deposit> GetDepositByCode(string codeBillet);
        Task<IEnumerable<Deposit>> ListAllBillets();
        Task SaveDeposit(Deposit deposit);
        Task UpdateDeposit(Deposit deposit);
    }
}
