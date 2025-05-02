using fusion.bank.admin.domain.Interfaces;
using fusion.bank.admin.domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.transfer.repository
{
    public class TransferRepository : ITransferRepository
    {
        private readonly IMongoCollection<BsonDocument> transferCollection;

        public TransferRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            transferCollection = dataBase.GetCollection<BsonDocument>("transferCollection");
        }

        public async Task<List<TransferSummary>> GetTransferSummaryAsync(string period)
        {
            var groupFormat = period.ToLower() switch
            {
                "week" => "%Y-%U", // Ano e semana
                "month" => "%Y-%m", // Ano e mês
                _ => "%Y-%m-%d" // Padrão: dia
            };

            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument
                        {
                            { "date", new BsonDocument("$dateToString", new BsonDocument
                                {
                                    { "format", groupFormat },
                                    { "date", "$transferDate" }
                                })
                            },
                            { "type", "$transferType" }
                        }
                    },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$_id.date" },
                    { "totalTransfers", new BsonDocument("$sum", "$count") },
                    { "details", new BsonDocument("$push", new BsonDocument
                        {
                            { "type", "$_id.type" },
                            { "count", "$count" }
                        })
                    }
                }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };

            var results = await transferCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var summaries = results.Select(doc => new TransferSummary
            {
                Period = doc["_id"].AsString,
                TotalTransfers = doc["totalTransfers"].AsInt32,
                PixCount = doc["details"].AsBsonArray
                    .Where(d => d["type"].AsString == "PIX")
                    .Sum(d => d["count"].AsInt32),
                DocCount = doc["details"].AsBsonArray
                    .Where(d => d["type"].AsString == "DOC")
                    .Sum(d => d["count"].AsInt32),
                TedCount = doc["details"].AsBsonArray
                    .Where(d => d["type"].AsString == "TED")
                    .Sum(d => d["count"].AsInt32)
            }).ToList();

            return summaries;
        }
    }
}
