using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        Task<Staff?> GetStaffByUserId(Guid userId);
    }
}
