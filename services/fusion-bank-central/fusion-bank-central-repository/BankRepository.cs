using fusion.bank.central.domain.Interfaces;
using fusion.bank.central.domain.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace fusion.bank.central.repository
{
    public class BankRepository : IBankRepository
    {
        private readonly IMongoCollection<Bank> bankCollection;

        public BankRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            bankCollection = dataBase.GetCollection<Bank>("bankCollection");
        }

        public async Task<Bank> ListBankByISPB(string id)
        {
            return (await bankCollection.FindAsync(d => d.ISPB == id)).FirstOrDefault();
        }

        public async Task<Bank> ListBankById(Guid id)
        {
            return (await bankCollection.FindAsync(d => d.BankId == id)).FirstOrDefault();
        }

        public async Task<Bank> ListAccountBankById(Guid id)
        {
            return (await bankCollection.FindAsync(d => d.Accounts.Exists(d => d.AccountId == id))).FirstOrDefault();
        }

        public async Task<IEnumerable<Bank>> ListAllBank()
        {
            return await bankCollection.AsQueryable().ToListAsync();
        }

        public async Task SaveBank(Bank account)
        {
            await bankCollection.InsertOneAsync(account);
        }

        public async Task UpdateBank(Bank account)
        {
            var filter = Builders<Bank>.Filter.Eq("BankId", account.BankId);

            await bankCollection.ReplaceOneAsync(filter, account);
        }
    }
}

