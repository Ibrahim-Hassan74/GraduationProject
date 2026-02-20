using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Drivers
{
    public interface IDriverService
    {
        Task<ApiResponse> GetDashboardAsync(Guid driverId);

        Task<ApiResponse> GetDriversBeforeMeAsync(Guid driverId);

        Task<ApiResponse> GetMyQueueAsync(Guid driverId);

        Task<ApiResponse> StartTripAsync(Guid driverId);

        Task<ApiResponse> EndTripAsync(Guid driverId);

        Task ResetDailyQueueAsync();
    }

}
