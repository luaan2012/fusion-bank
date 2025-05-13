namespace fusion.bank.deposit.domain.Requests
{
    public record DirectDeposit(Guid AccountId, string AccountNumberReceive, string AgencyNumberReceiver, decimal Amount, string Description = "");
}
