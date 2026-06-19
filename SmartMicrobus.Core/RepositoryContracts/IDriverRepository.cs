using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {
        Task<Driver?> GetDriverByLicense(string licenseNumber);
        Task<Driver?> GetDriverByPlateNumber(string plateNumber);
        Task<List<Driver>> GetDriversByStationAsync(Guid stationId);
    }
}
