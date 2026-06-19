namespace SmartMicrobus.Core.DTO.Microbus
{
    public class MicrobusDetailsResponse
    {
        public Guid Id { get; set; }

        public string PlateNumber { get; set; } = null!;

        public bool IsActive { get; set; }

        public int PassengerCount { get; set; }

        public string Model { get; set; } = null!;

        public string Color { get; set; } = null!;

        public string? QrCode { get; set; }

        public Guid RouteId { get; set; }

        public string RouteName { get; set; } = null!;

        public Guid? DriverId { get; set; }

        public string? DriverName { get; set; }
    }
}
