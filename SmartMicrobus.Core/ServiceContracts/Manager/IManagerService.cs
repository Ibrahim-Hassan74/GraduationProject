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
        Task<ApiResponse> GetStationDashboardAsync(Guid stationId);
        Task<ApiResponseWithData<byte[]>> ExportStationDataExcelAsync(Guid managerId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task <ApiResponseWithData<byte[]>> ExportStationRoutesExcelAsync(Guid managerId);
        Task<ApiResponseWithData<byte[]>> ExportStationDriversExcelAsync(Guid managerId);
        Task<ApiResponseWithData<byte[]>> ExportMicrobusesExcelAsync(Guid managerId);
        Task<ApiResponse> GetPaginatedStationMicrobusesAsync(MicrobusQuery query, Guid stationId);
        Task<ApiResponse> GetPaginatedStationDriversAsync(DriverQuery query, Guid stationId);

        // Staff CRUD
        Task<ApiResponse> AddStaffAsync(SmartMicrobus.Core.DTO.Staff.AddStaffDTO dto, Guid stationId);
        Task<ApiResponse> UpdateStaffAsync(Guid staffId, SmartMicrobus.Core.DTO.Staff.UpdateStaffDTO dto, Guid stationId);
        Task<ApiResponse> DeleteStaffAsync(Guid staffId, Guid stationId);
        Task<ApiResponse> GetPaginatedStationStaffAsync(SmartMicrobus.Core.DTO.Staff.StaffQuery query, Guid stationId);
    }
}
