using fusion.bank.core.Enum;
using fusion.bank.core.Events;
using fusion.bank.events.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace fusion.bank.events.repository
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<EventMessage> eventCollection;
        private NotificationType[] lastTransactions = [NotificationType.TRANSFER_MADE, NotificationType.TRANSFER_SCHEDULED, NotificationType.TRANSFER_RECEIVE, NotificationType.DEPOSIT];

        public EventRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            eventCollection = dataBase.GetCollection<EventMessage>(configuration["CollectionName"]);
        }

        public async Task<List<EventMessage>> ListEventById(string accountId, int limit = int.MaxValue)
        {
            return await eventCollection.Find(d => d.AccountId == accountId).Limit(limit).ToListAsync();
        }

        public async Task<List<EventMessage>> ListLastTransactions(string accountId, int limit = int.MaxValue)
        {
            var sort = Builders<EventMessage>.Sort.Descending(d => d.Date);

            var filter = Builders<EventMessage>.Filter.And(Builders<EventMessage>.Filter.Eq(e => e.AccountId, accountId),
                    Builders<EventMessage>.Filter.In(e => e.Action, lastTransactions));

            return await eventCollection.Find(filter)
                .Limit(limit)
                .Sort(sort)
                .ToListAsync();
        }

        public async Task SaveEvent(EventMessage eventMessage)
        {
            await eventCollection.InsertOneAsync(eventMessage);
        }
    }
}
