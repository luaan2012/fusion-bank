using fusion.bank.core.Events;
using fusion.bank.events.domain.Interfaces;
using MassTransit;

namespace fusion.bank.account.service
{
    public class EventConsumer(IEventRepository eventRepository) : IConsumer<EventMessage>
    {
        public async Task Consume(ConsumeContext<EventMessage> context)
        {
            await eventRepository.SaveEvent(context.Message);
        }
    }
}
