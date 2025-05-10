using fusion.bank.central.domain.Model;
using fusion.bank.central.domain.Request;

namespace fusion.bank.central.domain.Interfaces
{
    public interface IBankRepository
    {
        Task SaveBank(Bank account);
        Task<IEnumerable<Bank>> ListAllBank();
        Task<Bank> ListBankById(Guid id);
        Task<Bank> ListAccountBankById(Guid id);
        Task<Bank> ListBankByISPB(string id);
        Task<Bank> ListAccountBankByKeyAccount(string keyAccount);
        Task<Bank> ListAccountBankByAccountNumber(string accountNumber);
        Task<Bank> ListAccountBankByAccountAgencyNumber(string accountNumber, string agencyNumber);
        Task<bool> DeleteBank(Guid bankId);
        Task<Bank> UpdateBank(Guid bankId, BankEditRequest bankEditRequest);
        Task UpdateBank(Bank bank);
    }
}
