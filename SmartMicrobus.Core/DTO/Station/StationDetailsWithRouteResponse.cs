namespace SmartMicrobus.Core.DTO.Station
{
    public class StationDetailsWithRouteResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Address { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }

        public double DistanceKm { get; set; }
        public double EtaMinutes { get; set; }

        public List<List<double>> Points { get; set; } = new();
    }
}
