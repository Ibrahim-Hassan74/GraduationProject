namespace SmartMicrobus.Core.DTO.Microbus
{
    public class MicrobusListResponse
    {
        public Guid Id { get; set; }

        public string PlateNumber { get; set; } = null!;

        public bool IsActive { get; set; }

        public int PassengerCount { get; set; }

        public string? DriverName { get; set; }

        public string RouteName { get; set; } = null!;
    }
}
