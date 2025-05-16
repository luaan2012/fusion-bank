using fusion.bank.core.Enum;

namespace fusion.bank.deposit.domain.Requests
{
    public record DepositRequest(string Code, PaymentType PaymentType);
}
