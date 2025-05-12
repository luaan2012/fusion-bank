using fusion.bank.core.Events;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.events.domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.events.api.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class EventController(IEventRepository eventRepository) : MainController
    {
        [HttpGet("list-events/{accountId}")]
        public async Task<IActionResult> ListEventsById(string accountId, int limit)
        {
            limit = limit > 0 ? limit : int.MaxValue;

            var events = await eventRepository.ListEventById(accountId, limit);

            return CreateResponse(new DataContractMessage<List<EventMessage>>() { Data = events, Success = true });
        }

        [HttpGet("list-last-transactions/{accountId}")]
        public async Task<IActionResult> ListLastTransactions(string accountId, int limit)
        {
            var events = await eventRepository.ListLastTransactions(accountId, limit);

            return CreateResponse(new DataContractMessage<List<EventMessage>>() { Data = events, Success = true });
        }

        [HttpGet("mark-ready/{id}")]
        public async Task<IActionResult> ListLastTransactions(Guid id)
        {
            var success = await eventRepository.MarkReady(id);

            return CreateResponse(new DataContractMessage<List<EventMessage>>() { Success = success });
        }

        [HttpPost("mark-ready")]
        public async Task<IActionResult> MarkReadyAllByIds(Guid[] id)
        {
            var success = await eventRepository.MarkReadyAllByIds(id);

            return CreateResponse(new DataContractMessage<List<EventMessage>>() { Success = success });
        }
    }
}
