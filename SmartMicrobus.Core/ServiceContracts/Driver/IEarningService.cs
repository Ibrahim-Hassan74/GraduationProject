using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Driver
{
    public interface IEarningService
    {
        Task<ApiResponse> GetDailyEarningEstimationAsync(Guid driverId, Guid routeId, Guid stationId);
    }
}
