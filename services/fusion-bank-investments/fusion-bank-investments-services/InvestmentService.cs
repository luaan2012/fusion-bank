using System.Text.Json;
using fusion.bank.core.Enum;
using fusion.bank.core.Interfaces;
using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.domain.Models;

namespace fusion.bank.investments.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IRedisCacheService _redisCacheService;
        private const string AlphaVantageApiKey = "UIU5QBJA3BRJ8TMJ";
        private const string BcbSelicUrl = "https://api.bcb.gov.br/dados/serie/bcdata.sgs.11/dados/ultimos/1?formato=json";
        private const int CacheExpirationSeconds = 3600;

        public InvestmentService(HttpClient httpClient, IRedisCacheService redisCacheService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        public async Task<decimal> GetSelicRateAsync()
        {
            const string cacheKey = "selic_rate";
            var cachedValue = await _redisCacheService.GetAsync<decimal>(cacheKey);

            if (cachedValue != default)
            {
                Console.WriteLine($"Cache hit for key: {cacheKey} at {DateTime.Now}");
                return cachedValue;
            }

            try
            {
                var response = await _httpClient.GetStringAsync(BcbSelicUrl);
                var json = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(response);
                var selicRate = decimal.Parse(json[0]["valor"]) / 100;

                await _redisCacheService.SetAsync(cacheKey, selicRate, TimeSpan.FromSeconds(CacheExpirationSeconds));
                Console.WriteLine($"Cache miss for key: {cacheKey}, value stored: {selicRate} at {DateTime.Now}");
                return selicRate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Selic rate: {ex.Message} at {DateTime.Now}");
                return 0.105m; // Fallback to 10.5%
            }
        }

        public async Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            var cacheKey = $"price_{symbol}";
            var cachedPrice = await _redisCacheService.GetAsync<decimal>(cacheKey);

            if (cachedPrice != default)
            {
                Console.WriteLine($"Cache hit for key: {cacheKey} at {DateTime.Now}");
                return cachedPrice;
            }

            try
            {
                var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={AlphaVantageApiKey}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(response);
                var price = decimal.Parse(json["Global Quote"]["05. price"]);

                await _redisCacheService.SetAsync(cacheKey, price, TimeSpan.FromSeconds(CacheExpirationSeconds));
                Console.WriteLine($"Cache miss for key: {cacheKey}, value stored: {price} at {DateTime.Now}");
                return price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching price for symbol {symbol}: {ex.Message} at {DateTime.Now}");
                return 0;
            }
        }

        public async Task<List<AvailableInvestment>> GetAvailableInvestmentsAsync()
        {
            var investments = new List<AvailableInvestment>
            {
                // Ações (10)
                new AvailableInvestment { Symbol = "PETR4.SA", Type = InvestmentType.Stock, Name = "Petrobras PN" },
                new AvailableInvestment { Symbol = "VALE3.SA", Type = InvestmentType.Stock, Name = "Vale" },
                new AvailableInvestment { Symbol = "ITUB4.SA", Type = InvestmentType.Stock, Name = "Itaú Unibanco PN" },
                new AvailableInvestment { Symbol = "BBDC4.SA", Type = InvestmentType.Stock, Name = "Bradesco PN" },
                new AvailableInvestment { Symbol = "ABEV3.SA", Type = InvestmentType.Stock, Name = "Ambev" },
                new AvailableInvestment { Symbol = "MGLU3.SA", Type = InvestmentType.Stock, Name = "Magazine Luiza" },
                new AvailableInvestment { Symbol = "WEGE3.SA", Type = InvestmentType.Stock, Name = "Weg" },
                new AvailableInvestment { Symbol = "BBAS3.SA", Type = InvestmentType.Stock, Name = "Banco do Brasil" },
                new AvailableInvestment { Symbol = "GGBR4.SA", Type = InvestmentType.Stock, Name = "Gerdau PN" },
                new AvailableInvestment { Symbol = "SUZB3.SA", Type = InvestmentType.Stock, Name = "Suzano" },

                // FIIs (5)
                new AvailableInvestment { Symbol = "KNRI11.SA", Type = InvestmentType.FII, Name = "Kinea Renda Imobiliária" },
                new AvailableInvestment { Symbol = "HGLG11.SA", Type = InvestmentType.FII, Name = "CSHG Logística" },
                new AvailableInvestment { Symbol = "MXRF11.SA", Type = InvestmentType.FII, Name = "Maxi Renda" },
                new AvailableInvestment { Symbol = "VISC11.SA", Type = InvestmentType.FII, Name = "Vinci Shopping Centers" },
                new AvailableInvestment { Symbol = "XPML11.SA", Type = InvestmentType.FII, Name = "XP Malls" },

                // CDBs (5)
                new AvailableInvestment { Symbol = "CDB_108", Type = InvestmentType.CDB, Name = "CDB 108% Selic" },
                new AvailableInvestment { Symbol = "CDB_110", Type = InvestmentType.CDB, Name = "CDB 110% Selic" },
                new AvailableInvestment { Symbol = "CDB_115", Type = InvestmentType.CDB, Name = "CDB 115% Selic" },
                new AvailableInvestment { Symbol = "CDB_120", Type = InvestmentType.CDB, Name = "CDB 120% Selic" },
                new AvailableInvestment { Symbol = "CDB_125", Type = InvestmentType.CDB, Name = "CDB 125% Selic" },

                // LCIs (3)
                new AvailableInvestment { Symbol = "LCI_95", Type = InvestmentType.LCI, Name = "LCI 95% Selic" },
                new AvailableInvestment { Symbol = "LCI_100", Type = InvestmentType.LCI, Name = "LCI 100% Selic" },
                new AvailableInvestment { Symbol = "LCI_105", Type = InvestmentType.LCI, Name = "LCI 105% Selic" },

                // LCAs (2)
                new AvailableInvestment { Symbol = "LCA_95", Type = InvestmentType.LCA, Name = "LCA 95% Selic" },
                new AvailableInvestment { Symbol = "LCA_100", Type = InvestmentType.LCA, Name = "LCA 100% Selic" }
            };

            foreach (var inv in investments.Where(i => i.Type == InvestmentType.Stock || i.Type == InvestmentType.FII))
            {
                inv.CurrentPrice = await GetCurrentPriceAsync(inv.Symbol);
            }

            return investments;
        }
    }
}
 
