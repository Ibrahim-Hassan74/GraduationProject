using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; }

        public Guid DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }

        public TripStatus Status { get; set; }
    }

}
