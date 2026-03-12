namespace SmartMicrobus.Core.DTO.Common
{
    public class MicrobusQrPayload
    {
        public Guid MicrobusId { get; set; }
        public Guid DriverId { get; set; }
        public Guid RouteId { get; set; }
        public Guid QueueId { get; set; }
        public string DriverName { get; set; } = null!; 
        public string PlateNumber { get; set; } = null!;
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
