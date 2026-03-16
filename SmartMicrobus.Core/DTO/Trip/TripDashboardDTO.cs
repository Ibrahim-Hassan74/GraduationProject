namespace SmartMicrobus.Core.DTO.Trip
{
    public class TripDashboardDTO
    {
        public Guid TripId { get; set; }

        public string RouteFrom { get; set; } = null!;

        public string RouteTo { get; set; } = null!;

        public DateTimeOffset StartedAt { get; set; }

        public double DistanceKm { get; set; }

        public int EstimatedArrivalMinutes { get; set; }
    }
}
