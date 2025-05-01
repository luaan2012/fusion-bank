using fusion.bank.admin.domain.Interfaces;
using fusion.bank.core.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace fusion.bank.admin.repository
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
    }
}
