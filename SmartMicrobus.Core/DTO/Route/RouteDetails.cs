namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteDetails
    {
        public Guid Id { get; set; }
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public decimal Price { get; set; }
        public double DistanceKm { get; set; }
        public Guid FromStationId { get; set; }
        public Guid ToStationId { get; set; }
    }
}
