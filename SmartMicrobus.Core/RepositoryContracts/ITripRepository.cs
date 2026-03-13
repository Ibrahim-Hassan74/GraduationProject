using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Trip;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface ITripRepository : IGenericRepository<Trip>
    {
        Task<Trip?> GetActiveTripAsync(Guid driverId);
        Task<TripHistoryResponse> GetDriverTripsAsync(Guid driverId, DriverHistoryRequest request);
    }
}