using Microsoft.AspNetCore.SignalR;
using SmartMicrobus.API.Hubs;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.ServiceContracts.Common;

namespace SmartMicrobus.API.Realtime
{
    public class SignalRQueueNotificationService : IQueueNotificationService
    {
        private readonly IHubContext<DriverQueueHub> _hubContext;

        public SignalRQueueNotificationService(IHubContext<DriverQueueHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyDriverAdded(Guid queueId, QueueItemResponse driver)
        {
            await _hubContext.Clients
                .Group(queueId.ToString())
                .SendAsync("DriverAdded", driver);
        }

        public async Task NotifyDriverRemoved(Guid queueId, Guid driverId)
        {
            await _hubContext.Clients
                .Group(queueId.ToString())
                .SendAsync("DriverRemoved", driverId);
        }
    }
}
