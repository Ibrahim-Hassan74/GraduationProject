namespace SmartMicrobus.Core.DTO.Queue
{
    public class QueueItemResponse
    {
        public Guid DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public int Position { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
