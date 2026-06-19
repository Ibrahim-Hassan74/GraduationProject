using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Trip;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface ITripRepository : IGenericRepository<Trip>
    {
        Task<Trip?> GetActiveTripAsync(Guid driverId);
        Task<TripHistoryResponse> GetDriverTripsAsync(Guid driverId, DriverHistoryRequest request);
        Task<List<Trip>> GetMicrobusesOnTheWayAsync(Guid routeId);
        Task<int> GetMicrobusesOnTheWayCountAsync(Guid routeId);
        Task<List<Trip>> GetStationTripsAsync(Guid routeId, Guid stationId);
        Task<Trip?> GetTripByDriverIdAsync(Guid driverId);
        Task<List<Trip>> GetTripsByStationAndDateAsync(Guid stationId, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}