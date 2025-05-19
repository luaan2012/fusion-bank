using System.Text.Json.Serialization;

namespace fusion.bank.investments.domain.Response
{
    public class BrApiResponse
    {
        public IList<BrApiResult> Results { get; set; }
        public string RequestedAt { get; set; }
        public string Took { get; set; }
    }

    public class BrApiResult
    {
        public string? Currency { get; set; }
        public decimal? MarketCap { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public decimal? RegularMarketChange { get; set; }
        public decimal? RegularMarketChangePercent { get; set; }
        public DateTime RegularMarketTime { get; set; }
        public decimal? RegularMarketPrice { get; set; }
        public decimal? RegularMarketDayHigh { get; set; }
        public string? RegularMarketDayRange { get; set; }
        public decimal? RegularMarketDayLow { get; set; }
        public long? RegularMarketVolume { get; set; }
        public decimal? RegularMarketPreviousClose { get; set; }
        public decimal? RegularMarketOpen { get; set; }
        public string? FiftyTwoWeekRange { get; set; }
        public decimal? FiftyTwoWeekLow { get; set; }
        public decimal? FiftyTwoWeekHigh { get; set; }
        public string? Symbol { get; set; }
        public decimal? PriceEarnings { get; set; }
        public decimal? EarningsPerShare { get; set; }
        public string? Logourl { get; set; }
    }
}
