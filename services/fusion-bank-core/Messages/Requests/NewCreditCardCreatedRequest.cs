using fusion.bank.core.Model;

namespace fusion.bank.core.Messages.Requests
{
    public class NewCreditCardCreatedRequest
    {
        public CreditCard CreditCard { get; set; }
        public Guid AccountId { get; set; }
    };
}
