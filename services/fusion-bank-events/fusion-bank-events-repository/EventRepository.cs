using fusion.bank.core.Events;
using fusion.bank.events.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace fusion.bank.events.repository
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<EventMessage> eventCollection;

        public EventRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            eventCollection = dataBase.GetCollection<EventMessage>(configuration["CollectionName"]);
        }

        public async Task<EventMessage> ListEventById(string accountId)
        {
            return (await eventCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();
        }

        public async Task SaveEvent(EventMessage eventMessage)
        {
            await eventCollection.InsertOneAsync(eventMessage);
        }
    }
}
