using fusion.bank.core.Enum;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace fusion.bank.investments.domain.Models
{
    public class Investment
    {
        private readonly decimal InterestRate = 0.12M;

        [BsonId]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Balance { get; set; }
        public decimal PaidOff => CalculatePaidOff();
        public decimal TotalBalance => TotalBalanceHandle();
        public List<InvestmentEntry> Entries { get; set; } = [];

        [BsonRepresentation(BsonType.String)]
        public InvestmentType InvestmentType { get; set; }
        public DateTime DateInvestment { get; set; }

        public void AddInvestment(decimal amount, DateTime date)
        {
            Entries.Add(new InvestmentEntry
            {
                Amount = amount,
                Date = date
            });
        }

        private decimal CalculatePaidOff()
        {
            decimal totalPaidOff = 0;
            var dailyRate = InterestRate / 365;

            foreach (var entry in Entries.Where(d => decimal.IsPositive(d.Amount)))
            {
                var days = (DateTime.Now - entry.Date).TotalDays;
                switch (InvestmentType)
                {
                    case InvestmentType.CDB:
                    case InvestmentType.LCI:
                    case InvestmentType.LCA:
                        totalPaidOff += entry.Amount * (decimal)Math.Pow(1 + (double)dailyRate, days) - entry.Amount;
                        break;
                    default:
                        totalPaidOff += 0;
                        break;
                }
            }

            return totalPaidOff;
        }

        private decimal TotalBalanceHandle()
        {
            decimal totalInvested = Entries.Sum(entry => entry.Amount);
            return totalInvested + PaidOff;
        }

        public void UpdateBalance()
        {
            Balance = Entries.Sum(d => d.Amount);
        }
    }
}
