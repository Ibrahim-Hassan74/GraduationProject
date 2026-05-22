

using SmartMicrobus.Core.DTO.Route;

namespace SmartMicrobus.Core.ServiceContracts.Notification
{
    public interface ILocationBroadcastService
    {
        Task BroadcastDriverLocationAsync(Guid driverId, RouteResultDTO routeResult);
    }
}
