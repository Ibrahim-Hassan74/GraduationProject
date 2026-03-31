using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {
        Task<Driver?> GetDriverByPlateNumber(string plateNumber);
    }
}
