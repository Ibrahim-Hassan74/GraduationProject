
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IQueueItemRepository : IGenericRepository<QueueItem>
    {
        Task<QueueItem?> GetActiveByDriverIdAsync(Guid driverId);

        Task<bool> ExistsActiveByDriverIdAsync(Guid driverId);

        Task<int> GetLastPositionAsync(Guid queueId);

        Task<int> CountDriversBeforeAsync(Guid queueId, int position);

        Task<int> CountActiveAsync(Guid queueId);

        Task<QueueItem?> GetFirstInQueueAsync(Guid queueId);

        Task<List<QueueItem>> GetActiveQueueItemsAsync(Guid queueId);
        Task<List<QueueItem>> GetItemsBeforeAsync(Guid queueId, int currentPosition);
    }
}