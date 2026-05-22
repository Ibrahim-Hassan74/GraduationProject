using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartMicrobus.Core.Enums;
using System.Security.Claims;

namespace SmartMicrobus.API.Hubs
{
    [Authorize(Roles = $"{nameof(UserRole.Driver)}")]
    public class DriverDashboardHub : Hub
    {
        public async Task JoinDashboard()
        {
            var driverId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (driverId == null)
                throw new HubException("Unauthorized");

            await Groups.AddToGroupAsync(Context.ConnectionId, driverId);
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