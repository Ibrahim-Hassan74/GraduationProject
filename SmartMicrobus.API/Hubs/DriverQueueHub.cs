using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartMicrobus.Core.Enums;
using System.Security.Claims;

namespace SmartMicrobus.API.Hubs
{
    [Authorize(Roles = $"{nameof(UserRole.Driver)}")]
    public class DriverQueueHub : Hub
    {
        public async Task JoinQueueGroup(string queueId)
        {
            if (string.IsNullOrWhiteSpace(queueId))
                throw new HubException("QueueId is required.");

            await Groups.AddToGroupAsync(Context.ConnectionId, queueId);
        }
        public override async Task OnConnectedAsync()
        {
            var driverId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (driverId == null)
                throw new HubException("Unauthorized");

            await base.OnConnectedAsync();
        }
    }
}
