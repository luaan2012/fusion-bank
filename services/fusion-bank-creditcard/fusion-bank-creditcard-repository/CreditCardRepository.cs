using fusion.bank.core.Model;
using fusion.bank.creditcard.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
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
            await creditCardCollection.DeleteOneAsync(d => d.AccountId == creditCard.AccountId);

            await creditCardCollection.InsertOneAsync(creditCard);
        }

        public async Task<CreditCard> GetTriedCard(Guid accountId)
        {
            return (await creditCardCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();
        }

        public async Task<IEnumerable<CreditCard>> ListAllCreditCards()
        {
            var filter = new BsonDocument();
            return (await creditCardCollection.FindAsync(filter)).ToList();
        }
    }
}

