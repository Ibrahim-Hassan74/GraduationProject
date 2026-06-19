using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Microbus;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.DTO.Staff;

namespace SmartMicrobus.Core.ServiceContracts.Manager
{
    public interface IManagerService
    {
        Task<ApiResponse> AddDriverAsync(DriverAddRequest driverAddRequest);
        Task<ApiResponse> AddMicrobusAsync(MicrobusAddRequest microbusAddRequest);
        Task<ApiResponse> GetManagerStationAsync(Guid managerId);
        Task<ApiResponse> AssignDriverToMicrobusAsync(DriverAssignRequest driverAssignRequest);
        Task<ApiResponse> GetStationDashboardAsync(Guid stationId);
        Task<ApiResponseWithData<byte[]>> ExportStationDataExcelAsync(Guid stationId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task <ApiResponseWithData<byte[]>> ExportStationRoutesExcelAsync(Guid stationId);
        Task<ApiResponseWithData<byte[]>> ExportStationDriversExcelAsync(Guid stationId);
        Task<ApiResponseWithData<byte[]>> ExportMicrobusesExcelAsync(Guid stationId);
        Task<ApiResponseWithData<byte[]>> ExportReportsExcelAsync(GetReportsQuery query, Guid stationId);
        Task<ApiResponse> GetPaginatedStationMicrobusesAsync(MicrobusQuery query, Guid stationId);
        Task<ApiResponse> GetPaginatedStationDriversAsync(DriverQuery query, Guid stationId);
        

        // Staff CRUD
        Task<ApiResponse> AddStaffAsync(AddStaffDTO dto, Guid stationId);
        Task<ApiResponse> UpdateStaffAsync(Guid staffId, UpdateStaffDTO dto, Guid stationId);
        Task<ApiResponse> DeleteStaffAsync(Guid staffId, Guid stationId);
        Task<ApiResponse> GetPaginatedStationStaffAsync(StaffQuery query, Guid stationId);
    }
}
