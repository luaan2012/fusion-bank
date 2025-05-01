namespace fusion.bank.deposit.domain.Requests
{
    public class BilletRequest
    {
        public Guid AccountId { get; set; }
        public string SwiftCode { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
    }
}
