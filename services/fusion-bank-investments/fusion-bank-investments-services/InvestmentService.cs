using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using fusion.bank.core.Enum;
using fusion.bank.core.Interfaces;
using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.domain.Models;
using fusion.bank.investments.domain.Response;

namespace fusion.bank.investments.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IRedisCacheService _redisCacheService;
        private const string AlphaVantageApiKey = "wDt8UU5JaJ6PzmbkFFaKHT";
        private const string BcbSelicUrl = "https://api.bcb.gov.br/dados/serie/bcdata.sgs.11/dados/ultimos/1?formato=json";
        private const int CacheExpirationSeconds = 3600;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

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
                // Converter taxa percentual diária (e.g., "0.054266") para decimal (e.g., 0.00054266)
                var selicRate = decimal.Parse(json[0]["valor"], CultureInfo.InvariantCulture) / 100;

                await _redisCacheService.SetAsync(cacheKey, selicRate, TimeSpan.FromSeconds(CacheExpirationSeconds));
                Console.WriteLine($"Cache miss for key: {cacheKey}, value stored: {selicRate} at {DateTime.Now}");
                return selicRate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Selic rate: {ex.Message} at {DateTime.Now}");
                return 0.00054266m; // Fallback: 0.054266% diário (baseado no exemplo)
            }
        }

        public async Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            var cacheKey = $"investment_{symbol}";
            var cachedInvestment = await _redisCacheService.GetAsync<AvailableInvestment>(cacheKey);

            if (cachedInvestment != null)
            {
                Console.WriteLine($"Cache hit for key: {cacheKey} at {DateTime.Now}");
                return cachedInvestment.RegularMarketPrice;
            }

            try
            {
                var url = $"https://brapi.dev/api/quote/{symbol}?token={AlphaVantageApiKey}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JsonSerializer.Deserialize<BrApiResponse>(response, _jsonOptions);

                var result = json.Results.FirstOrDefault();

                if (result != null)
                {
                    var availableInvestment = new AvailableInvestment(result);

                    await _redisCacheService.SetAsync(cacheKey, availableInvestment, TimeSpan.FromSeconds(CacheExpirationSeconds));
                    Console.WriteLine($"Cache miss for key: {cacheKey}, value stored at {DateTime.Now}");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data for symbol {symbol}: {ex.Message} at {DateTime.Now}");
                return 0;
            }
        }

        public async Task UpdateInvestmentAsync(AvailableInvestment investment)
        {
            var cacheKey = $"investment_{investment.Symbol}";
            var cachedInvestment = await _redisCacheService.GetAsync<AvailableInvestment>(cacheKey);

            if (cachedInvestment != null)
            {
                investment.UpdateInvesment(cachedInvestment);
                return;
            }

            try
            {
                var url = $"https://brapi.dev/api/quote/{investment.Symbol}?token={AlphaVantageApiKey}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JsonSerializer.Deserialize<BrApiResponse>(response, _jsonOptions);

                var result = json.Results.FirstOrDefault();
                if (result != null)
                {
                    investment.UpdateInvesment(result);

                    await _redisCacheService.SetAsync(cacheKey, investment, TimeSpan.FromSeconds(CacheExpirationSeconds));
                    Console.WriteLine($"Cache miss for key: {cacheKey}, value stored at {DateTime.Now}");
                }
                else
                {
                    Console.WriteLine($"No data returned for symbol {investment.Symbol} at {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data for symbol {investment.Symbol}: {ex.Message} at {DateTime.Now}");
            }
        }

        public async Task<List<AvailableInvestment>> GetAvailableInvestmentsAsync()
        {
            var investments = new List<AvailableInvestment>
            {
                // Ações (150)
                new AvailableInvestment { Symbol = "PETR4", Type = InvestmentType.STOCK, ShortName = "Petrobras PN" },
                new AvailableInvestment { Symbol = "VALE3", Type = InvestmentType.STOCK, ShortName = "Vale" },
                new AvailableInvestment { Symbol = "ITUB4", Type = InvestmentType.STOCK, ShortName = "Itaú Unibanco PN" },
                new AvailableInvestment { Symbol = "BBDC4", Type = InvestmentType.STOCK, ShortName = "Bradesco PN" },
                new AvailableInvestment { Symbol = "ABEV3", Type = InvestmentType.STOCK, ShortName = "Ambev" },
                new AvailableInvestment { Symbol = "MGLU3", Type = InvestmentType.STOCK, ShortName = "Magazine Luiza" },
                new AvailableInvestment { Symbol = "WEGE3", Type = InvestmentType.STOCK, ShortName = "Weg" },
                new AvailableInvestment { Symbol = "BBAS3", Type = InvestmentType.STOCK, ShortName = "Banco do Brasil" },
                new AvailableInvestment { Symbol = "GGBR4", Type = InvestmentType.STOCK, ShortName = "Gerdau PN" },
                new AvailableInvestment { Symbol = "SUZB3", Type = InvestmentType.STOCK, ShortName = "Suzano" },
                new AvailableInvestment { Symbol = "RENT3", Type = InvestmentType.STOCK, ShortName = "Localiza" },
                new AvailableInvestment { Symbol = "LREN3", Type = InvestmentType.STOCK, ShortName = "Lojas Renner" },
                new AvailableInvestment { Symbol = "JBSS3", Type = InvestmentType.STOCK, ShortName = "JBS" },
                new AvailableInvestment { Symbol = "CSNA3", Type = InvestmentType.STOCK, ShortName = "CSN" },
                new AvailableInvestment { Symbol = "BRFS3", Type = InvestmentType.STOCK, ShortName = "BRF" },
                new AvailableInvestment { Symbol = "CMIG4", Type = InvestmentType.STOCK, ShortName = "Cemig PN" },
                new AvailableInvestment { Symbol = "ELET3", Type = InvestmentType.STOCK, ShortName = "Eletrobras" },
                new AvailableInvestment { Symbol = "SANB11", Type = InvestmentType.STOCK, ShortName = "Santander Brasil" },
                new AvailableInvestment { Symbol = "B3SA3", Type = InvestmentType.STOCK, ShortName = "B3" },
                new AvailableInvestment { Symbol = "PRIO3", Type = InvestmentType.STOCK, ShortName = "PetroRio" },
                new AvailableInvestment { Symbol = "TOTS3", Type = InvestmentType.STOCK, ShortName = "Totvs" },
                new AvailableInvestment { Symbol = "HAPV3", Type = InvestmentType.STOCK, ShortName = "Hapvida" },
                // ... (mais ações até 150, consultar lista completa na B3 se necessário)

                // FIIs (35)
                new AvailableInvestment { Symbol = "KNRI11", Type = InvestmentType.FII, ShortName = "Kinea Renda Imobiliária" },
                new AvailableInvestment { Symbol = "HGLG11", Type = InvestmentType.FII, ShortName = "CSHG Logística" },
                new AvailableInvestment { Symbol = "MXRF11", Type = InvestmentType.FII, ShortName = "Maxi Renda" },
                new AvailableInvestment { Symbol = "VISC11", Type = InvestmentType.FII, ShortName = "Vinci Shopping Centers" },
                new AvailableInvestment { Symbol = "XPML11", Type = InvestmentType.FII, ShortName = "XP Malls" },
                new AvailableInvestment { Symbol = "BRCR11", Type = InvestmentType.FII, ShortName = "BC Fund" },
                new AvailableInvestment { Symbol = "HGBS11", Type = InvestmentType.FII, ShortName = "Hedge Brasil Shopping" },
                new AvailableInvestment { Symbol = "JSRE11", Type = InvestmentType.FII, ShortName = "JS Real Estate" },
                new AvailableInvestment { Symbol = "KNCR11", Type = InvestmentType.FII, ShortName = "Kinea Rendimentos" },
                new AvailableInvestment { Symbol = "HFOF11", Type = InvestmentType.FII, ShortName = "Hedge FoF" },
                // ... (mais FIIs até 35, consultar lista na B3)
            };

            // Atualizar preços apenas para ações e FIIs
            foreach (var inv in investments.Where(i => i.Type == InvestmentType.STOCK || i.Type == InvestmentType.FII))
            {
                await UpdateInvestmentAsync(inv);
            }

            investments.AddRange([
                 // CDBs (5)
                new AvailableInvestment { Symbol = "CDB_108", Type = InvestmentType.CDB, ShortName = "CDB 108% Selic", RegularMarketPrice = 1 },
                new AvailableInvestment { Symbol = "CDB_110", Type = InvestmentType.CDB, ShortName = "CDB 110% Selic", RegularMarketPrice = 1 },
                new AvailableInvestment { Symbol = "CDB_115", Type = InvestmentType.CDB, ShortName = "CDB 115% Selic", RegularMarketPrice = 1 },
                new AvailableInvestment { Symbol = "CDB_120", Type = InvestmentType.CDB, ShortName = "CDB 120% Selic", RegularMarketPrice = 1 },
                new AvailableInvestment { Symbol = "CDB_125", Type = InvestmentType.CDB, ShortName = "CDB 125% Selic", RegularMarketPrice = 1 },

                // LCIs (3)
                new AvailableInvestment { Symbol = "LCI_95", Type = InvestmentType.LCI, ShortName = "LCI 95% Selic", RegularMarketPrice = 1 },
                new AvailableInvestment { Symbol = "LCI_100", Type = InvestmentType.LCI, ShortName = "LCI 100% Selic", RegularMarketPrice = 1 },
                new AvailableInvestment { Symbol = "LCI_105", Type = InvestmentType.LCI, ShortName = "LCI 105% Selic", RegularMarketPrice = 1 },

                // LCAs (2)
                new AvailableInvestment { Symbol = "LCA_95", Type = InvestmentType.LCA, ShortName = "LCA 95% Selic", RegularMarketPrice = 1 },
                new AvailableInvestment { Symbol = "LCA_100", Type = InvestmentType.LCA, ShortName = "LCA 100% Selic", RegularMarketPrice = 1 }
            ]);

            return investments;
        }
    }
}

