using Microsoft.AspNetCore.SignalR;
using SmartMicrobus.API.Hubs;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.ServiceContracts.Notification;

namespace SmartMicrobus.API.Realtime
{
    public class SignalRDashboardNotificationService : IDashboardNotificationService
    {
        private readonly IHubContext<DriverDashboardHub> _hubContext;

        public SignalRDashboardNotificationService(
            IHubContext<DriverDashboardHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendDashboard(Guid driverId, DriverDashboardDTO dashboard)
        {
            await _hubContext.Clients
                .Group(driverId.ToString())
                .SendAsync("DashboardUpdated", dashboard);
        }
    }
}