using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IReportReasonRepository: IGenericRepository<ReportReason>
    {
        Task<List<ReportReason>> GetByIdsAsync(List<int> ids);
    }
}
