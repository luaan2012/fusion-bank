using Microsoft.AspNetCore.SignalR;

namespace fusion.bank.events.services
{
    public class EventHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
        }

        public async Task LeaveUserGroup(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
        }
    }
}
