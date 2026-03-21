using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Trip : BaseEntity<Guid>
    {
        public Guid DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public Guid MicrobusId { get; set; }
        public Microbus Microbus { get; set; } = null!;

        public Guid StationId { get; set; }
        public Station Station { get; set; } = null!;
        
        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }

        public int PassengerCount { get; set; }
        public double DistanceKm { get; set; }
        public decimal TotalAmount { get; set; }

        public TripStatus Status { get; set; }
    }
}
