using fusion.bank.admin.domain.Models;
using fusion.bank.core.Model;

namespace fusion.bank.admin.domain.Interfaces
{
    public interface IEventRepository
    {
        Task<List<EventSummary>> GetRecentEventsAsync(int limit = 10);
        Task<PagedEventResult<EventSummary>> GetAllEventsPagedAsync(int pageNumber, int pageSize);
    }
}
