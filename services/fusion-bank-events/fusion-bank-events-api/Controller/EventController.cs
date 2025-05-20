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

        [HttpPut("mark-ready/{id}")]
        public async Task<IActionResult> ListLastTransactions(Guid id)
        {
            var success = await eventRepository.MarkReady(id);

            return CreateResponse(new DataContractMessage<List<EventMessage>>() { Success = success }, "Successo");
        }

        [HttpPut("mark-all-ready/{ids}")]
        public async Task<IActionResult> MarkReadyAllByIds(Guid[] ids)
        {
            var success = await eventRepository.MarkReadyAllByIds(ids);

            return CreateResponse(new DataContractMessage<List<EventMessage>>() { Success = success }, "Sucesso");
        }

        [HttpDelete("delete-all-by-id/{accountId}")]
        public async Task<IActionResult> DeleteAllById(string accountId)
        {
            var success = await eventRepository.DeleteAllById(accountId);

            return CreateResponse(new DataContractMessage<List<EventMessage>>() { Success = success }, "Sucesso");
        }
    }
}
