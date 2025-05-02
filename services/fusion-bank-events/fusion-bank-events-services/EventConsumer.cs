using fusion.bank.core.Events;
using fusion.bank.events.domain.Interfaces;
using fusion.bank.events.services;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace fusion.bank.account.service
{
    public class EventConsumer(IEventRepository eventRepository, IHubContext<EventHub> hubContext) : IConsumer<EventMessage>
    {
        public async Task Consume(ConsumeContext<EventMessage> context)
        {
            await eventRepository.SaveEvent(context.Message);

            await hubContext.Clients.Group(context.Message.AccountId.ToString()).SendAsync("ReceiveNotification", context.Message);
        }
    }
}
