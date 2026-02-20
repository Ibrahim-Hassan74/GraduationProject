namespace SmartMicrobus.Core.DTO.Queue
{
    public class QueueItemDTO
    {
        public Guid DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public int Position { get; set; }
        public string Status { get; set; } = null!;
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
