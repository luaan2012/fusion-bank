using fusion.bank.admin.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace fusion.deposit.admin.repository
{
    public class DepositRepository : IDepositRepository
    {
        //private readonly IMongoCollection<Deposit> depositCollection;

        //public DepositRepository(IConfiguration configuration)
        //{
        //    var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
        //    var dataBase = client.GetDatabase("fusion_db");
        //    depositCollection = dataBase.GetCollection<Deposit>("depositCollection");
        //}
    }
}

