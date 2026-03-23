using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IReportRepository : IGenericRepository<DriverReport>
    {
        Task<DriverReport?> GetByIdWithReasonsAsync(Guid id);
    }
}
