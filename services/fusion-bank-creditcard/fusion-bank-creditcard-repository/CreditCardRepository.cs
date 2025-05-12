using fusion.bank.core.Model;
using fusion.bank.creditcard.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.central.repository
{
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly IMongoCollection<CreditCard> creditCardCollection;

        public CreditCardRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            creditCardCollection = dataBase.GetCollection<CreditCard>(configuration["CollectionName"]);
        }

        public async Task SaveTriedCard(CreditCard creditCard)
        {
            await creditCardCollection.DeleteOneAsync(d => d.AccountId == creditCard.AccountId);

            await creditCardCollection.InsertOneAsync(creditCard);
        }

        public async Task<CreditCard> ListCreditCardById(Guid id)
        {
            return (await creditCardCollection.FindAsync(d => d.Id == id)).FirstOrDefault();
        }

        public async Task<CreditCard> ListCreditCardByAccountId(Guid accountId)
        {
            return (await creditCardCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();
        }

        public async Task<IEnumerable<CreditCard>> ListAllCreditCards()
        {
            var filter = new BsonDocument();
            return (await creditCardCollection.FindAsync(filter)).ToList();
        }

        public async Task<bool> ToggleBlockdCard(Guid id, bool isBlocked)
        {
            var filter = Builders<CreditCard>.Filter.Eq(d => d.Id, id);
            var update = Builders<CreditCard>.Update.Set(d => d.CreditCardBlocked, isBlocked);

            var response = await creditCardCollection.UpdateOneAsync(filter, update);

            return response.ModifiedCount > 0;
        }

        public async Task<bool> VirtaulCreditCardDelete(Guid id)
        {
            var filter = Builders<CreditCard>.Filter.Eq(d => d.Id, id);
            var update = Builders<CreditCard>.Update.Set(d => d.VirtualCreditCards, []);

            var response = await creditCardCollection.UpdateOneAsync(filter, update);

            return response.ModifiedCount > 0;
        }
    }
}

