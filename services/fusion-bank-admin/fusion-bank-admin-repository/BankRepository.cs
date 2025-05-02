using fusion.bank.admin.domain.Interfaces;
using fusion.bank.admin.domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.admin.repository
{
    public class BankRepository : IBankRepository
    {
        private readonly IMongoCollection<BsonDocument> bankCollection;

        public BankRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            bankCollection = dataBase.GetCollection<BsonDocument>("bankCollection");
        }

        public async Task<BankSummary> GetBankSummaryAsync()
        {
            // Pipeline para agregação
            var pipeline = new[]
            {
                // Agrupar por banco para contar contas e somar saldos
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$bankCode" },
                    { "accountCount", new BsonDocument("$sum", 1) },
                    { "totalBalance", new BsonDocument("$sum", "$balance") }
                }),
                // Agrupar novamente para obter totais gerais e detalhes por banco
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", null },
                    { "totalAccounts", new BsonDocument("$sum", "$accountCount") },
                    { "totalBalanceAllBanks", new BsonDocument("$sum", "$totalBalance") },
                    { "bankDetails", new BsonDocument("$push", new BsonDocument
                        {
                            { "bankCode", "$_id" },
                            { "accountCount", "$accountCount" },
                            { "totalBalance", "$totalBalance" }
                        })
                    }
                })
            };

            // Executar pipeline
            var result = await bankCollection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            // Criar objeto de resumo
            var summary = new BankSummary
            {
                TotalAccounts = result != null ? result["totalAccounts"].AsInt32 : 0,
                TotalBalanceAllBanks = result != null ? result["totalBalanceAllBanks"].AsDouble : 0,
                BankDetails = result != null && result["bankDetails"].IsBsonArray
                    ? result["bankDetails"].AsBsonArray.Select(doc => new BankDetail
                    {
                        BankCode = doc["bankCode"].AsString,
                        AccountCount = doc["accountCount"].AsInt32,
                        TotalBalance = doc["totalBalance"].AsDouble
                    }).ToList()
                    : new List<BankDetail>()
            };

            return summary;
        }

        public async Task<List<BankListItem>> GetBankListAsync()
        {
            var banks = await bankCollection
                .Find(Builders<BsonDocument>.Filter.Empty)
                .ToListAsync();

            var bankList = banks.Select(doc => new BankListItem
            {
                BankName = doc["name"].AsString,
                IspbCode = doc["bankCode"].AsString,
                ActiveAccounts = doc["activeAccounts"].AsInt32
            }).ToList();

            return bankList;
        }

        public async Task<List<BankAccountItem>> GetBankAccountsAsync(string bankCode)
        {
            var accounts = await bankCollection
                .Aggregate()
                .Match(new BsonDocument("bankCode", bankCode))
                .ToListAsync();

            var accountList = accounts.Select(doc => new BankAccountItem
            {
                Holder = doc["holder"].AsString,
                CpfCnpj = doc["cpfCnpj"].AsString,
                AgencyAccount = doc["agencyAccount"].AsString,
                Balance = doc["balance"].AsDouble,
                Limit = doc["limit"].AsDouble,
                Status = doc["status"].AsString
            }).ToList();

            return accountList;
        }

        public async Task<bool> UpdateBankAsync(string bankCode, BankUpdateModel updateModel)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("bankCode", bankCode);
            var update = Builders<BsonDocument>.Update
                .Set("name", updateModel.Name)
                .Set("activeAccounts", updateModel.ActiveAccounts);

            var result = await bankCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}

