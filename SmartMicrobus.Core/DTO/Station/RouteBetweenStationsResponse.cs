namespace SmartMicrobus.Core.DTO.Station
{
    public class RouteBetweenStationsResponse
    {
        public Guid FromStationId { get; set; }
        public string FromName { get; set; } = null!;
        public double FromLat { get; set; }
        public double FromLng { get; set; }

        public Guid ToStationId { get; set; }
        public string ToName { get; set; } = null!;
        public double ToLat { get; set; }
        public double ToLng { get; set; }

        public double DistanceKm { get; set; }
        public double EtaMinutes { get; set; }

        public List<List<double>> Points { get; set; } = new();
    }
}
