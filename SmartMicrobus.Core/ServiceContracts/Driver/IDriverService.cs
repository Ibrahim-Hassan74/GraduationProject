using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.ServiceContracts.Drivers
{
    public interface IDriverService
    {
        Task<ApiResponseWithData<DriverResponse>> GetDriverByIdAsync(Guid driverId);
        Task<ApiResponseWithData<DriverResponse>> GetDriverByLicenseAsync(string licenseNumber);
        Task<ApiResponseWithData<DriverDashboardDTO>> GetDashboardAsync(Guid driverId);

        Task<ApiResponseWithData<int>> GetDriversBeforeMeAsync(Guid driverId);

        Task<ApiResponseWithData<IEnumerable<QueueItemResponse>>> GetMyQueueAsync(Guid driverId);

        Task ResetDailyQueueAsync();
        Task<ApiResponse> GetDriverByPlateNumber(string plateNumber);
    }

}
