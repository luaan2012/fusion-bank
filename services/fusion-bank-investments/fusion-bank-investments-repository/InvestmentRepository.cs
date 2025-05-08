using fusion.bank.investments.domain.Interfaces;
using fusion.bank.investments.domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.investments.repository
{
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly IMongoCollection<Investment> investmentCollection;

        public InvestmentRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            investmentCollection = dataBase.GetCollection<Investment>(configuration["CollectionName"]);
        }

        public async Task<Investment> GetInvestmentById(Guid guid, Guid accountId)
        {
            return (await investmentCollection.FindAsync(d => d.Id == guid && d.AccountId == accountId)).FirstOrDefault();
        }

        public async Task<List<Investment>> ListInvestmentByAccountId(Guid accountId, int limit)
        {
            var filter = Builders<Investment>.Filter.Eq(e => e.AccountId, accountId);

            return await investmentCollection.Find(filter)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<List<Investment>> GetAllInvestment()
        {
            var filter = new BsonDocument();

            return (await investmentCollection.FindAsync(filter)).ToList();
        }

        public async Task SaveInvestment(Investment investment)
        {
            await investmentCollection.InsertOneAsync(investment);
        }

        public async Task Update(Investment investment)
        {
            await investmentCollection.DeleteOneAsync(d => d.AccountId == investment.AccountId);

            await investmentCollection.InsertOneAsync(investment);
        }

        public async Task DeleteInvestmentById(Guid guid)
        {
            await investmentCollection.DeleteOneAsync(d => d.Id == guid);
        }
    }
}

