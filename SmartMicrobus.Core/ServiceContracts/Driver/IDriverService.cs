using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.ServiceContracts.Drivers
{
    public interface IDriverService
    {
        Task<ApiResponseWithData<DriverDashboardDTO>> GetDashboardAsync(Guid driverId);

        Task<ApiResponseWithData<int>> GetDriversBeforeMeAsync(Guid driverId);

        Task<ApiResponseWithData<IEnumerable<QueueItemResponse>>> GetMyQueueAsync(Guid driverId);

        Task<ApiResponse> StartTripAsync(Guid driverId);

        Task<ApiResponse> EndTripAsync(Guid driverId);

        Task ResetDailyQueueAsync();
        Task<ApiResponse> GetDriverHistoryAsync(Guid driverId, DriverHistoryRequest request);
        Task<ApiResponse> GetDriverByPlateNumber(string plateNumber);
    }

}
