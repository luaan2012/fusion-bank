using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.account.repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account> accountCollection;

        public AccountRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            accountCollection = dataBase.GetCollection<Account>("accountCollection");
        }


        public async Task<Account> ListAccountById(Guid id)
        {
            return (await accountCollection.FindAsync(d => d.AccountId == id)).FirstOrDefault();
        }

        public async Task<Account> ListAccountByKey(string keyAccount)
        {
            return (await accountCollection.FindAsync(d => d.KeyAccount == keyAccount)).FirstOrDefault();
        }

        public async Task<IEnumerable<Account>> ListAllAccount()
        {
            var filter = new BsonDocument();
            return (await accountCollection.FindAsync(filter)).ToList();
        }

        public async Task SaveAccount(Account account)
        {
            await accountCollection.InsertOneAsync(account);
        }

        public async Task SaveKeyByAccount(Guid idAccount, string keyAccount)
        {
            var filter = Builders<Account>.Filter.Eq(d => d.AccountId, idAccount);
            var update = Builders<Account>.Update.Set(d => d.KeyAccount, keyAccount);

            await accountCollection.UpdateOneAsync(filter, update);
        }


        public async Task UpdateAccount(Account account)
        {
            var filter = Builders<Account>.Filter.Eq("AccountId", account.AccountId);

            await accountCollection.ReplaceOneAsync(filter, account);
        }
    }
}
