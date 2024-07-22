using fusion.bank.core.Enum;

namespace fusion.bank.core.Messages.Producers
{
    public record NewAccountProducer(Guid AccountId, string Name, string LastName, string FullName, string AccountNumber,
                                        decimal Balance, decimal TransferLimit, decimal SalaryPerMonth, AccountType AccountType, string BankISBP, string keyAccount);
}
