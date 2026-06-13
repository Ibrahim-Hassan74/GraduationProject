using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Report;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IReportRepository : IGenericRepository<DriverReport>
    {
        Task<DriverReport?> GetByIdWithReasonsAsync(Guid id);
        Task<DriverReport?> GetByIdWithReasonsAsync(Guid id, Guid stationId);
        Task<bool> HasRecentReportAsync(Guid passengerId, string plate);
        Task<(List<DriverReport> Items, int TotalCount)> GetPagedReportsAsync(Guid passengerId, GetReportsQuery query);
        Task<(List<DriverReport> Items, int TotalCount)> GetPagedReportsForAdminAsync(GetReportsQuery query, Guid stationId);
    }
}
