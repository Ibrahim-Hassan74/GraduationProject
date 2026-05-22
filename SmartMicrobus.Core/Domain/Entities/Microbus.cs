namespace SmartMicrobus.Core.Domain.Entities
{
    public class Microbus : BaseEntity<Guid>
    {
        public string PlateNumber { get; set; } = null!;

        public string? QrCode { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public Guid? DriverId { get; set; }
        public virtual Driver? Driver { get; set; }

        public int PassengerCount { get; set; }
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}
