using fusion.bank.core.Enum;

namespace fusion.bank.investments.domain.Models
{
    public class InvestmentRequest
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public InvestmentType InvestmenType { get; set; }
    }
}
