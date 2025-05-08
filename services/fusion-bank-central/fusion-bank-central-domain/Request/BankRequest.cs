using fusion.bank.central.domain;
using fusion.bank.central.domain.Enum;

namespace fusion.bank.central.Request
{
    public class BankRequest
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public decimal Fee { get; set; }
        public BankType BankType { get; set; }
        public AdvantageType[] AdvantageType { get; set; }
    }
}
