using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.Enums;
using System.Security.Claims;

namespace SmartMicrobus.API.Hubs
{
   
    [Authorize(Roles = $"{nameof(UserRole.Passenger)}")]
    public class LocationTrackingHub : Hub
    {
        private const string DriverGroupPrefix = "driver-";

        
        public async Task JoinDriver(Guid driverId)
        {
            if (driverId == Guid.Empty)
                throw new HubException("Invalid driver ID");

            var groupName = GetDriverGroup(driverId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        
        public async Task LeaveDriver(Guid driverId)
        {
            if (driverId == Guid.Empty)
                throw new HubException("Invalid driver ID");

            var groupName = GetDriverGroup(driverId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

       
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new HubException("Unauthorized: No user identity found");

            await base.OnConnectedAsync();
        }

        
        private static string GetDriverGroup(Guid driverId) => $"{DriverGroupPrefix}{driverId}";

       
      
    }
}
