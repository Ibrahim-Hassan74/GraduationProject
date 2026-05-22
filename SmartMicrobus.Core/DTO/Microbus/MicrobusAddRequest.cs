namespace SmartMicrobus.Core.DTO.Microbus
{
    public class MicrobusAddRequest
    {
        public string PlateNumber { get; set; } = null!;
        public Guid RouteId { get; set; }
        public int PassengerCount { get; set; }
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}
