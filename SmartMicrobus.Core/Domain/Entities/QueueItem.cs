

using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class QueueItem
    {
        public Guid Id { get; set; }

        public Guid QueueId { get; set; }
        public Queue Queue { get; set; } = null!;

        public Guid DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public int Position { get; set; }

        public QueueStatus Status { get; set; } = QueueStatus.Waiting;

        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    }

}
