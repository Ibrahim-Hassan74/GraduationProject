using SmartMicrobus.Core.DTO.Route;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverResponse
    {
        public string To { get; set; } = null!;
        public string From { get; set; } = null!;
        public int PassengerCount { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public Guid DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string PlateNumber { get; set; }
        public string LicenseNumber { get; set; }
    }
}
