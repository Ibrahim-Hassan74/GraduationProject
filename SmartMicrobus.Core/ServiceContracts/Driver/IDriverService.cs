using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Driver
{
    public interface IDriverService
    {
        Task<ApiResponse> GetDashboardAsync(Guid driverId);

        Task<ApiResponse> GetDriversBeforeMeAsync(Guid driverId);

        Task<ApiResponse> GetQueueByRouteAsync(Guid stationId, Guid routeId);

        Task<ApiResponse> StartTripAsync(Guid driverId);

        Task<ApiResponse> EndTripAsync(Guid driverId);

        Task<ApiResponse> ResetDailyQueueAsync();
    }

}
