namespace SmartMicrobus.Core.Domain.Entities
{
    public class Microbus : BaseEntity<Guid>
    {
        public string PlateNumber { get; set; } = null!;

        public string QrCode { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public Guid DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public int PassengerCount { get; set; }
    }
}
