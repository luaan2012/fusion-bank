using fusion.bank.core.Enum;
using fusion.bank.deposit.domain;

namespace fusion.bank.core.Messages.Requests
{
    public record TransactionRequest(Guid AccountId, Guid DepositId, string AccountReceiver, string AgencyReceiver, decimal Amount, ExpenseCategory ExpenseCategory, PaymentType PaymentType, string Description = "");
}
