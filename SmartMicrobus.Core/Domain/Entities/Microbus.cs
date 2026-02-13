

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Microbus
    {
        public Guid Id { get; set; }

        public string PlateNumber { get; set; } = null!;

        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public Guid DriverId { get; set; }
        public Driver Driver { get; set; } = null!;
    }

}
