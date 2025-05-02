using fusion.bank.core.Events;

namespace fusion.bank.events.domain.Interfaces
{
    public interface IEventRepository
    {
        Task<EventMessage> ListEventById(string guid);
        Task SaveEvent(EventMessage eventMessage);
    }
}
