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
        private NotificationType[] lastTransactions = [NotificationType.TRANSFER_MADE,
            NotificationType.TRANSFER_SCHEDULED, NotificationType.TRANSFER_RECEIVE, 
            NotificationType.DEPOSIT_CREATE, NotificationType.DEPOSIT_MADE, 
            NotificationType.DEPOSIT_DIRECT_MADE, NotificationType.BILLET_MADE];

        public EventRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            eventCollection = dataBase.GetCollection<EventMessage>(configuration["CollectionName"]);
        }

        public async Task<List<EventMessage>> ListEventById(string accountId, int limit)
        {
            var sort = Builders<EventMessage>.Sort.Descending(d => d.Date);
            return await eventCollection.Find(d => d.AccountId == accountId).Limit(limit).Sort(sort).ToListAsync();
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

        public async Task<bool> MarkReady(Guid id)
        {
            var filter = Builders<EventMessage>.Filter.Eq(e => e.EventId, id);
            var update = Builders<EventMessage>.Update.Set(d => d.Read, true);

            var response = await eventCollection.UpdateOneAsync(filter, update);

            return response.ModifiedCount > 0;
        }

        public async Task<bool> MarkReadyAllByIds(Guid[] id)
        {
            var filter = Builders<EventMessage>.Filter.In(e => e.EventId, id);
            var update = Builders<EventMessage>.Update.Set(d => d.Read, true);

            var response = await eventCollection.UpdateManyAsync(filter, update);

            return response.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAllById(string accountId)
        {
            var filter = Builders<EventMessage>.Filter.Eq(e => e.AccountId, accountId);

            var response = await eventCollection.DeleteManyAsync(filter);

            return response.DeletedCount > 0;
        }

        public async Task SaveEvent(EventMessage eventMessage)
        {
            await eventCollection.InsertOneAsync(eventMessage);
        }
    }
}
