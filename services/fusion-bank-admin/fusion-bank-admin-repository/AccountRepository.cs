using fusion.bank.admin.domain;
using fusion.bank.admin.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.admin.repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<BsonDocument> accountCollection;

        public AccountRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            accountCollection = dataBase.GetCollection<BsonDocument>("accountCollection");
        }

        public async Task<AccountRegistrationSummary> GetAccountRegistrationSummaryAsync()
        {
            // Data atual e limite para últimas 24 horas
            var now = DateTime.UtcNow;
            var last24Hours = now.AddHours(-24);

            // Pipeline para contar cadastros nas últimas 24 horas
            var last24HoursPipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "createdAt", new BsonDocument
                        {
                            { "$gte", last24Hours }
                        }
                    }
                }),
                new BsonDocument("$count", "total")
            };

            // Pipeline para contar cadastros por mês no ano atual
            var monthlyPipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "createdAt", new BsonDocument
                        {
                            { "$gte", new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                            { "$lte", now }
                        }
                    }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument("$month", "$createdAt") },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };

            // Executar pipelines
            var last24HoursResult = await accountCollection.Aggregate<BsonDocument>(last24HoursPipeline).FirstOrDefaultAsync();
            var monthlyResults = await accountCollection.Aggregate<BsonDocument>(monthlyPipeline).ToListAsync();

            // Inicializar array com 12 zeros (jan-dez)
            var monthlyCounts = new int[12];
            foreach (var result in monthlyResults)
            {
                var month = result["_id"].AsInt32 - 1; // MongoDB $month retorna 1-12, array usa 0-11
                monthlyCounts[month] = result["count"].AsInt32;
            }

            // Criar objeto de resumo
            var summary = new AccountRegistrationSummary
            {
                TotalLast24Hours = last24HoursResult != null ? last24HoursResult["total"].AsInt32 : 0,
                MonthlyRegistrations = monthlyCounts
            };

            return summary;
        }

    }
}
