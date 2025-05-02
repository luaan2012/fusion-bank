using fusion.bank.admin.domain.Interfaces;
using fusion.bank.admin.domain.Models;
using fusion.bank.core.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.transfer.repository
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<BsonDocument> eventCollection;

        public EventRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            eventCollection = dataBase.GetCollection<BsonDocument>("eventCollection");
        }

        public async Task<List<EventSummary>> GetRecentEventsAsync(int limit = 10)
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            var sort = Builders<BsonDocument>.Sort.Descending("eventDate");

            var events = await eventCollection
                .Find(filter)
                .Sort(sort)
                .Limit(limit)
                .ToListAsync();

            var summaries = events.Select(doc => new EventSummary
            {
                EventDate = doc["eventDate"].AsBsonDateTime.ToUniversalTime(),
                Action = doc["action"].AsString,
                User = doc["user"].AsString,
                Amount = doc.Contains("amount") && !doc["amount"].IsBsonNull ? doc["amount"].AsDouble : 0
            }).ToList();

            return summaries;
        }

        public async Task<PagedEventResult<EventSummary>> GetAllEventsPagedAsync(int pageNumber, int pageSize)
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            var sort = Builders<BsonDocument>.Sort.Descending("eventDate");

            // Contar total de documentos
            var totalCount = await eventCollection.CountDocumentsAsync(filter);

            // Calcular informações de paginação
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var skip = (pageNumber - 1) * pageSize;

            // Buscar eventos paginados
            var events = await eventCollection
                .Find(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            var eventSummaries = events.Select(doc => new EventSummary
            {
                EventDate = doc["eventDate"].AsBsonDateTime.ToUniversalTime(),
                Action = doc["action"].AsString,
                User = doc["user"].AsString,
                Amount = doc.Contains("amount") && !doc["amount"].IsBsonNull ? doc["amount"].AsDouble : 0
            }).ToList();

            return new PagedEventResult<EventSummary>
            {
                Data = eventSummaries,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalEvents = (int)totalCount
            };
        }
    }
}
