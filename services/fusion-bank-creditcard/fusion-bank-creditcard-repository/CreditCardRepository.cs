using fusion.bank.creditcard.domain.Interfaces;
using fusion.bank.creditcard.domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace fusion.bank.central.repository
{
    public class CreditCardRepository : ICreditCartRepository
    {
        private readonly IMongoCollection<CreditCard> creditCardCollection;

        public CreditCardRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            creditCardCollection = dataBase.GetCollection<CreditCard>("CreditCardCollection");
        }

        public async Task SaveTriedCard(CreditCard creditCard)
        {
            await creditCardCollection.InsertOneAsync(creditCard);
        }

        public async Task<CreditCard> SaveTriedCard(Guid accountId)
        {
            return (await creditCardCollection.FindAsync<CreditCard>(d => d.AccountId == accountId)).FirstOrDefault();
        }
    }
}

