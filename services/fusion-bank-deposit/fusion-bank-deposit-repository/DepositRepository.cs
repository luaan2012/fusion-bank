using fusion.bank.deposit.domain.Interfaces;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using fusion.bank.deposit.domain.Models;

namespace fusion.deposit.deposit.repository
{
    public class DepositRepository : IDepositRepository
    {
        private readonly IMongoCollection<Deposit> depositCollection;

        public DepositRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            depositCollection = dataBase.GetCollection<Deposit>("depositCollection");
        }

        public async Task<Deposit> GetDepositById(Guid id)
        {
            return (await depositCollection.FindAsync(d => d.DepositId == id)).FirstOrDefault();
        }

        public async Task<IEnumerable<Deposit>> ListAllBillets()
        {
            return await depositCollection.AsQueryable().ToListAsync();
        }

        public async Task SaveDeposit(Deposit deposit)
        {
            await depositCollection.InsertOneAsync(deposit);
        }

        public async Task UpdateDeposit(Deposit deposit)
        {
            var filter = Builders<Deposit>.Filter.Eq(d => d.DepositId, deposit.DepositId);

            await depositCollection.ReplaceOneAsync(filter, deposit);
        }
    }
}

