using SmartMicrobus.Core.ServiceContracts.Drivers;
using SmartMicrobus.Core.ServiceContracts.Notification;

namespace SmartMicrobus.Core.Services.Common
{
    public class DriverDashboardRealtimeService
    {
        private readonly IDriverService _driverService;
        private readonly IDashboardNotificationService _notificationService;

        public DriverDashboardRealtimeService(
            IDriverService driverService,
            IDashboardNotificationService notificationService)
        {
            _driverService = driverService;
            _notificationService = notificationService;
        }

        public async Task PushDashboard(Guid driverId)
        {
            var response = await _driverService.GetDashboardAsync(driverId);

            if (response.Data != null)
            {
                await _notificationService.SendDashboard(driverId, response.Data);
            }
        }
    }
}
