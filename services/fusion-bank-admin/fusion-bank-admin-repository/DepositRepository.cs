using fusion.bank.admin.domain.Interfaces;
using fusion.bank.admin.domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.deposit.admin.repository
{
    public class DepositRepository : IDepositRepository
    {

        private readonly IMongoCollection<BsonDocument> depositCollection;

        public DepositRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            depositCollection = dataBase.GetCollection<BsonDocument>("depositCollection");
        }

        public async Task<BilletsSummary> GetBoletoSummaryAsync()
        {
            // Pipeline para boletos gerados (total e valor somado)
            var totalBoletoPipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("type", "BOLETO")),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", null },
                    { "totalGenerated", new BsonDocument("$sum", 1) },
                    { "totalAmount", new BsonDocument("$sum", "$amount") }
                })
            };

            // Pipeline para boletos por status (pago, vencido, a vencer)
            var statusBoletoPipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("type", "BOLETO")),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$status" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            // Executar pipelines
            var totalBoletoResult = await depositCollection.Aggregate<BsonDocument>(totalBoletoPipeline).FirstOrDefaultAsync();
            var statusBoletoResults = await depositCollection.Aggregate<BsonDocument>(statusBoletoPipeline).ToListAsync();

            // Inicializar contadores de status
            int paidCount = 0, overdueCount = 0, pendingCount = 0;
            foreach (var result in statusBoletoResults)
            {
                var status = result["_id"].AsString;
                var count = result["count"].AsInt32;
                switch (status.ToUpper())
                {
                    case "PAID":
                        paidCount = count;
                        break;
                    case "OVERDUE":
                        overdueCount = count;
                        break;
                    case "PENDING":
                        pendingCount = count;
                        break;
                }
            }

            // Criar objeto de resumo
            var summary = new BilletsSummary
            {
                TotalGenerated = totalBoletoResult != null ? totalBoletoResult["totalGenerated"].AsInt32 : 0,
                TotalAmount = totalBoletoResult != null ? totalBoletoResult["totalAmount"].AsDouble : 0,
                StatusCounts = new int[] { paidCount, overdueCount, pendingCount } // [pago, vencido, a vencer]
            };

            return summary;
        }

        public async Task<DepositSummary> GetDepositSummaryAsync()
        {
            // Pipeline para depósitos (quantidade e valor total)
            var depositPipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("type", "DEPOSIT")),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", null },
                    { "totalDeposits", new BsonDocument("$sum", 1) },
                    { "totalAmount", new BsonDocument("$sum", "$amount") }
                })
            };

            // Executar pipeline
            var depositResult = await depositCollection.Aggregate<BsonDocument>(depositPipeline).FirstOrDefaultAsync();

            // Criar objeto de resumo
            var summary = new DepositSummary
            {
                TotalDeposits = depositResult != null ? depositResult["totalDeposits"].AsInt32 : 0,
                TotalAmount = depositResult != null ? depositResult["totalAmount"].AsDouble : 0
            };

            return summary;
        }
    }
}

