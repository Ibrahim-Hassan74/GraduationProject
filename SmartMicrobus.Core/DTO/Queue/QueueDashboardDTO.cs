namespace SmartMicrobus.Core.DTO.Queue
{
    public class QueueDashboardDTO
    {
        public Guid QueueId { get; set; }

        public int Position { get; set; }

        public int DriversBefore { get; set; }

        public int TotalDrivers { get; set; }

        public string RouteFrom { get; set; } = null!;

        public string RouteTo { get; set; } = null!;
    }
}
