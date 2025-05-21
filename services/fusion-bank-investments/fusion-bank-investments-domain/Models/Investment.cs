using fusion.bank.core.Enum;
using fusion.bank.investments.domain.Response;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.investments.domain.Models
{
    public class Investment
    {

        [BsonId]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Balance => CalculateBalance();
        public decimal PaidOff { get; set; }
        public decimal TotalBalance => CalculateTotalBalance();
        public decimal PercentageChange => CalculatePercentageChange();
        public List<InvestmentEntry> Entries { get; set; } = [];
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CurrentMarketValue { get; set; }
        public string Symbol { get; set; }
        public string ShortName { get; set; }
        public string Logourl { get; set; }

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

        public void RescueInvestment(decimal amount, DateTime date, decimal quantity = 0, decimal unitPrice = 0)
        {
            Entries.Add(new InvestmentEntry
            {
                Amount = -amount,
                Quantity = -quantity,
                UnitPrice = unitPrice,
                Date = date,
            });
            Quantity -= quantity;
            UnitPrice = unitPrice;
        }

        private decimal CalculateTotalBalance()
        {
            return Balance + PaidOff;
        }

        private decimal CalculateBalance()
        {
            return Entries.Sum(e => e.Amount);
        }

        private decimal CalculatePercentageChange()
        {
            var balance = CalculateBalance();
            if (balance == 0m)
            {
                return 0m; // Avoid division by zero
            }
            return (PaidOff / balance) * 100m; // Percentage change
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
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public decimal MarketCap { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public decimal RegularMarketChange { get; set; }
        public decimal RegularMarketChangePercent { get; set; }
        public DateTime RegularMarketTime { get; set; }
        public decimal RegularMarketPrice { get; set; }
        public decimal RegularMarketDayHigh { get; set; }
        public string RegularMarketDayRange { get; set; }
        public decimal RegularMarketDayLow { get; set; }
        public decimal RegularMarketVolume { get; set; }
        public decimal RegularMarketPreviousClose { get; set; }
        public decimal RegularMarketOpen { get; set; }
        public string FiftyTwoWeekRange { get; set; }
        public int FiftyTwoWeekLow { get; set; }
        public decimal FiftyTwoWeekHigh { get; set; }
        public string Symbol { get; set; }
        public decimal PriceEarnings { get; set; }
        public decimal EarningsPerShare { get; set; }
        public string Logourl { get; set; }
        public bool OnMyPocket { get; set; }
        public InvestmentType Type { get; set; }

        public AvailableInvestment()
        {
        }

        public AvailableInvestment(BrApiResult brApiResult)
        {
            Currency = brApiResult.Currency;
            MarketCap = brApiResult?.MarketCap ?? 0;
            ShortName = brApiResult.ShortName;
            LongName = brApiResult.LongName;
            RegularMarketChange = brApiResult?.RegularMarketChange ?? 0;
            RegularMarketChangePercent = brApiResult?.RegularMarketChangePercent ?? 0;
            RegularMarketDayHigh = brApiResult?.RegularMarketDayHigh ?? 0;
            RegularMarketDayLow = brApiResult?.RegularMarketDayLow ?? 0;
            RegularMarketTime = brApiResult.RegularMarketTime;
            Logourl = brApiResult.Logourl;
            EarningsPerShare = brApiResult?.EarningsPerShare ?? 0;
            PriceEarnings = brApiResult?.PriceEarnings ?? 0;
            FiftyTwoWeekHigh = brApiResult?.FiftyTwoWeekHigh ?? 0;
            FiftyTwoWeekRange = brApiResult.FiftyTwoWeekRange;
            RegularMarketOpen = brApiResult?.RegularMarketOpen ?? 0;
            RegularMarketPreviousClose = brApiResult?.RegularMarketPreviousClose ?? 0;
            RegularMarketVolume = brApiResult?.RegularMarketVolume ?? 0;
            RegularMarketDayRange = brApiResult.RegularMarketDayRange;
            RegularMarketPrice = brApiResult?.RegularMarketPrice ?? 0;
        }

        public void UpdateInvesment(BrApiResult brApiResult)
        {
            Currency = brApiResult.Currency;
            MarketCap = brApiResult?.MarketCap ?? 0;
            ShortName = brApiResult.ShortName;
            LongName = brApiResult.LongName;
            RegularMarketChange = brApiResult?.RegularMarketChange ?? 0;
            RegularMarketChangePercent = brApiResult?.RegularMarketChangePercent ?? 0;
            RegularMarketDayHigh = brApiResult?.RegularMarketDayHigh ?? 0;
            RegularMarketDayLow = brApiResult?.RegularMarketDayLow ?? 0;
            RegularMarketTime = brApiResult.RegularMarketTime;
            Logourl = brApiResult.Logourl;
            EarningsPerShare = brApiResult?.EarningsPerShare ?? 0;
            PriceEarnings = brApiResult?.PriceEarnings ?? 0;
            FiftyTwoWeekHigh = brApiResult?.FiftyTwoWeekHigh ?? 0;
            FiftyTwoWeekRange = brApiResult.FiftyTwoWeekRange;
            RegularMarketOpen = brApiResult?.RegularMarketOpen ?? 0;
            RegularMarketPreviousClose = brApiResult?.RegularMarketPreviousClose ?? 0;
            RegularMarketVolume = brApiResult?.RegularMarketVolume ?? 0;
            RegularMarketDayRange = brApiResult.RegularMarketDayRange;
            RegularMarketPrice = brApiResult?.RegularMarketPrice ?? 0;
        }

        public void UpdateInvesment(AvailableInvestment availableInvestment)
        {
            Currency = availableInvestment.Currency;
            MarketCap = availableInvestment.MarketCap;
            ShortName = availableInvestment.ShortName;
            LongName = availableInvestment.LongName;
            RegularMarketChange = availableInvestment.RegularMarketChange;
            RegularMarketChangePercent = availableInvestment.RegularMarketChangePercent;
            RegularMarketDayHigh = availableInvestment.RegularMarketDayHigh;
            RegularMarketDayLow = availableInvestment.RegularMarketDayLow;
            RegularMarketTime = availableInvestment.RegularMarketTime;
            Logourl = availableInvestment.Logourl;
            EarningsPerShare = availableInvestment.EarningsPerShare;
            PriceEarnings = availableInvestment.PriceEarnings;
            FiftyTwoWeekHigh = availableInvestment.FiftyTwoWeekHigh;
            FiftyTwoWeekRange = availableInvestment.FiftyTwoWeekRange;
            RegularMarketOpen = availableInvestment.RegularMarketOpen;
            RegularMarketPreviousClose = availableInvestment.RegularMarketPreviousClose;
            RegularMarketVolume = availableInvestment.RegularMarketVolume;
            RegularMarketDayRange = availableInvestment.RegularMarketDayRange;
            RegularMarketPrice = availableInvestment.RegularMarketPrice;
        }
    }
}
