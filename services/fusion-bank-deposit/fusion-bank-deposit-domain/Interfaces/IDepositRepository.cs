namespace fusion.bank.deposit.domain.Interfaces
{
    public interface IDepositRepository
    {
        Task<Deposit> GetDepositById(Guid id);
        Task<IEnumerable<Deposit>> ListAllBillets();
        Task SaveDeposit(Deposit deposit);
        Task UpdateDeposit(Deposit deposit);
    }
}
