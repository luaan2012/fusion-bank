using fusion.bank.deposit.domain;

namespace fusion.bank.core.Messages.Requests
{
    public record CreditCardTransactionRequest(Guid AccountId, decimal Amount, ExpenseCategory ExpenseCategory, string Description);
}
