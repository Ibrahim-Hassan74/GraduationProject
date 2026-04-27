namespace SmartMicrobus.Core.DTO.Station
{
    public class NearestStationResponse
    {
        public Guid StationId { get; set; }
        public string StationName { get; set; }
        public string StationAddress { get; set; }
        public string StationCity { get; set; }
        public double StationLat { get; set; }
        public double StationLng { get; set; }

        public double DistanceKm { get; set; }
        public double EtaMinutes { get; set; }

        public List<List<double>> Points { get; set; }
    }
}
