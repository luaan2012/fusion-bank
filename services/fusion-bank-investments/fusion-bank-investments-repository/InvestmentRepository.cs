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
            investmentCollection = dataBase.GetCollection<Investment>("investmentCollection");
        }

        public async Task<Investment> GetInvestmentById(Guid guid)
        {
            return (await investmentCollection.FindAsync(d => d.Id == guid)).FirstOrDefault();
        }

        public async Task<Investment> GetInvestmentByAccountId(Guid guid)
        {
            return (await investmentCollection.FindAsync(d => d.AccountId == guid)).FirstOrDefault();
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

