using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IQueueRepository : IGenericRepository<Queue>
    {
        Task<Queue?> GetByStationAndRouteAsync(Guid stationId, Guid routeId);
        Task<Queue?> GetByIdWithItemsAsync(Guid queueId);
        Task<Queue> GetOrCreateQueueAsync(Guid routeId, Guid stationId);
    }
}