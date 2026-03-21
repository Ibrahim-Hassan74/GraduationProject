using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;

namespace SmartMicrobus.Core.ServiceContracts.Driver
{
    public interface ITripService
    {
        Task<ApiResponse> StartTripAsync(Guid driverId);

        Task<ApiResponse> EndTripAsync(Guid driverId);

        Task<ApiResponse> GetDriverHistoryAsync(Guid driverId, DriverHistoryRequest request);
    }
}
