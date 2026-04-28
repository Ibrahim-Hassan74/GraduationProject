using Microsoft.AspNetCore.SignalR;
using SmartMicrobus.API.Hubs;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.ServiceContracts.Notification;

namespace SmartMicrobus.API.Realtime
{
    public class LocationBroadcastService: ILocationBroadcastService
    {
        private readonly IHubContext<LocationTrackingHub> _hubContext;

        public LocationBroadcastService(IHubContext<LocationTrackingHub> hubContext)
        {
            _hubContext = hubContext;
        }

  
        public async Task BroadcastDriverLocationAsync(Guid driverId, RouteResultDTO routeResult)
        {
            var groupName = $"driver-{driverId}";

            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveLocation", routeResult);
        }
    }
}
