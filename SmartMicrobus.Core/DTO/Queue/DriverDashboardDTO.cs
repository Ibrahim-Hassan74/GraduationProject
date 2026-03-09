namespace SmartMicrobus.Core.DTO.Queue
{
    public class DriverDashboardDTO
    {
        public Guid DriverId { get; set; }
        public Guid QueueId { get; set; }
        public string Status { get; set; } = null!;

        public int? Position { get; set; }

        public int? DriversBefore { get; set; }

        public int? TotalDrivers { get; set; }

        public string? RouteFrom { get; set; }

        public string? RouteTo { get; set; }
    }
}
