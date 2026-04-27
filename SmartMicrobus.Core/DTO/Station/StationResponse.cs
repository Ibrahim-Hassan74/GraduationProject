namespace SmartMicrobus.Core.DTO.Station
{
    public class StationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
