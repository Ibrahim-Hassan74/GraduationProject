namespace SmartMicrobus.Core.DTO.Queue
{
    public class QueueItemResponseDTO
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid MicrobusId { get; set; }
        public int Position { get; set; }
        public string DriverDisplayName { get; set; } = string.Empty;
        public string QueueType { get; set; } = string.Empty; // "Active" or "Waiting"
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset JoinedAt { get; set; }
    }
}