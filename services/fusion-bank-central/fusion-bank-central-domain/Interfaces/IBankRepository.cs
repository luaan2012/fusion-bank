using fusion.bank.central.domain.Model;

namespace fusion.bank.central.domain.Interfaces
{
    public interface IBankRepository
    {
        Task SaveBank(Bank account);
        Task<IEnumerable<Bank>> ListAllBank();
        Task<Bank> ListBankById(Guid id);
        Task<Bank> ListAccountBankById(Guid id);
        Task<Bank> ListBankByISPB(string id);
        Task UpdateBank(Bank account);
    }
}
