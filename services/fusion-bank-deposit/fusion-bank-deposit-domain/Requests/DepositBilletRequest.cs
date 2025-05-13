namespace fusion.bank.deposit.domain.Requests
{
    public record DepositBilletRequest(Guid AccountId, bool IsDeposit, string ISPB, decimal Amount, string NameReceiver, 
        string AccountNumberReceiver, string AgencyNumberReceiver, string DocumentReceiver, BilletType BilletType, string Description = "");
}

