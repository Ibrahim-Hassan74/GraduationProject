using SmartMicrobus.Core.DTO.Driver;

namespace SmartMicrobus.Core.ServiceContracts.Notification
{
    public interface IDashboardNotificationService
    {
        Task SendDashboard(Guid driverId, DriverDashboardDTO dashboard);
    }
}
