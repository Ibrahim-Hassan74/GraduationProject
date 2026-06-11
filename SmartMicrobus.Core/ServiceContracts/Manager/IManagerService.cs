using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Microbus;

namespace SmartMicrobus.Core.ServiceContracts.Manager
{
    public interface IManagerService
    {
        Task<ApiResponse> AddDriverAsync(DriverAddRequest driverAddRequest);
        Task<ApiResponse> AddMicrobusAsync(MicrobusAddRequest microbusAddRequest);
        Task<ApiResponse> GetManagerStationAsync(Guid managerId);
        Task<ApiResponse> AssignDriverToMicrobusAsync(DriverAssignRequest driverAssignRequest);
    }
}
