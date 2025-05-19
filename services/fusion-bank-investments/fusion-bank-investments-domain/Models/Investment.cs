using fusion.bank.core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.investments.domain.Models
{
    public class Investment
    {

        [BsonId]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Balance { get; set; }
        public decimal PaidOff { get; set; }
        public decimal TotalBalance { get; set; }
        public List<InvestmentEntry> Entries { get; set; } = [];
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CurrentMarketValue { get; set; }
        public string Symbol { get; set; }
        [BsonRepresentation(BsonType.String)]
        public InvestmentType InvestmentType { get; set; }
        public DateTime DateInvestment { get; set; }

        public void AddInvestment(decimal amount, DateTime date, decimal quantity = 0, decimal unitPrice = 0)
        {
            Entries.Add(new InvestmentEntry
            {
                Amount = amount,
                Date = date,
                Quantity = quantity,
                UnitPrice = unitPrice
            });
            Quantity += quantity;
            UnitPrice = unitPrice;
        }
    }

    public class InvestmentEntry
    {
        public decimal Amount { get; set; } // Valor da entrada
        public DateTime Date { get; set; } // Data da entrada
        public decimal Quantity { get; set; } // Quantidade (para ações/FIIs)
        public decimal UnitPrice { get; set; } // Preço unitário
    }

    public class AvailableInvestment
    {
        public string Symbol { get; set; }
        public InvestmentType Type { get; set; }
        public string Name { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
