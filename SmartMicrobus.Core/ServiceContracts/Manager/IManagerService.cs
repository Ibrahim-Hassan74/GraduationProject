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
        Task<ApiResponseWithData<byte[]>> ExportStationDataExcelAsync(Guid managerId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<ApiResponseWithData<byte[]>> ExportStationDriversExcelAsync(Guid managerId);
        Task<ApiResponseWithData<byte[]>> ExportStationRoutesExcelAsync(Guid managerId);
        Task<ApiResponseWithData<byte[]>> ExportMicrobusesExcelAsync(Guid managerId);
    }
}
