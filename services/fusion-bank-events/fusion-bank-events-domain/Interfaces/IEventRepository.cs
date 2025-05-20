using fusion.bank.core.Events;

namespace fusion.bank.events.domain.Interfaces
{
    public interface IEventRepository
    {
        Task<List<EventMessage>> ListEventById(string accountId, int limit);
        Task<List<EventMessage>> ListLastTransactions(string accountId, int limit = int.MaxValue);
        Task<bool> MarkReady(Guid id);
        Task<bool> MarkReadyAllByIds(Guid[] id);
        Task<bool> DeleteAllById(string accountId);
        Task SaveEvent(EventMessage eventMessage);
    }
}
