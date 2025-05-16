namespace fusion.bank.core.Messages.Requests
{
    public record TransactionCentralRequest(Guid AccountId, decimal BalanceReceiver = 0, decimal BalancePayer = 0, 
        string AccountPayer = "", string AgencyPayer = "", string AccountReceiver = "", string AgencyReceiver = "", bool UpdatePayer = false, bool UpdateReceiver = false);
}
