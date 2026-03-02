using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.ServiceContracts.Common
{
    public interface IQueueNotificationService
    {
        Task NotifyDriverAdded(Guid queueId, QueueItemResponse driver);
        Task NotifyDriverRemoved(Guid queueId, Guid driverId);
    }
}
