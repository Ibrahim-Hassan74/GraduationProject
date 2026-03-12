using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface ITripRepository : IGenericRepository<Trip>
    {
        Task<Trip?> GetActiveTripAsync(Guid driverId);
        Task<List<Trip>?> GetDriverTripsAsync(Guid driverId, DateTime from, DateTime to);
    }
}