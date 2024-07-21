namespace fusion.bank.core.Messages.Requests
{

    public class NewDepositCentralRequest
    {
        public NewDepositCentralRequest(Guid accountId, Guid depositId, string accountNumber, decimal amount)
        {
            AccountId = accountId;
            DepositId = depositId;
            AccountNumber = accountNumber;
            Amount = amount;
        }

        public NewDepositCentralRequest()
        {

        }

        public Guid AccountId { get; set; }
        public Guid DepositId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
