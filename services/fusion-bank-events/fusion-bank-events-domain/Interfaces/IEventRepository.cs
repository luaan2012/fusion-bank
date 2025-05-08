using fusion.bank.core.Events;
using fusion.bank.events.domain.Request;

namespace fusion.bank.events.domain.Interfaces
{
    public interface IEventRepository
    {
        Task<List<EventMessage>> ListEventById(string accountId, int limit);
        Task<List<EventMessage>> ListLastTransactions(string accountId, int limit = int.MaxValue);
        Task SaveEvent(EventMessage eventMessage);
    }
}
