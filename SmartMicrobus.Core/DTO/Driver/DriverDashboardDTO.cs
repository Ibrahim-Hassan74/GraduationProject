using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.DTO.Trip;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverDashboardDTO
    {
        public Guid DriverId { get; set; }

        public string Status { get; set; } = null!;

        public QueueDashboardDTO? Queue { get; set; }

        public TripDashboardDTO? Trip { get; set; }
    }
}
