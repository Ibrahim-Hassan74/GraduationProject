using Microsoft.AspNetCore.SignalR;

namespace SmartMicrobus.API.Hubs
{
    public class RouteTrackingHub : Hub
    {
        public async Task JoinRoute(Guid routeId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"route-{routeId}"
            );
        }

        public async Task LeaveRoute(Guid routeId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"route-{routeId}"
            );
        }
    }
}
