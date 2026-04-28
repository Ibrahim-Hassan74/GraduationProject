namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverLocationResponse
    {
        public Guid DriverId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
