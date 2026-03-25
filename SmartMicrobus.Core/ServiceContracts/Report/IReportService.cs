using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Report;

namespace SmartMicrobus.Core.ServiceContracts.Report
{
    public interface IReportService
    {
        Task<ApiResponse> CreateReportAsync(Guid passengerId, CreateReportRequest request);
        Task<ApiResponse> GetReasonsAsync();
        Task<ApiResponse> GetReportsAsync(Guid passengerId, GetReportsQuery query);
        Task<ApiResponse> GetReportByIdAsync(Guid passengerId, Guid reportId);
        Task<ApiResponse> UpdateReportAsync(Guid passengerId, Guid reportId, UpdateReportRequest request);
        Task<ApiResponse> DeleteReportAsync(Guid passengerId, Guid reportId);
    }
}
