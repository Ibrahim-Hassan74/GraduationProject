using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Report;

namespace SmartMicrobus.Core.ServiceContracts.Report
{
    public interface IReportService
    {
        Task<ApiResponse> CreateReportAsync(Guid passengerId, CreateReportRequest request);
        Task<ApiResponse> GetReasonsAsync();
    }
}
