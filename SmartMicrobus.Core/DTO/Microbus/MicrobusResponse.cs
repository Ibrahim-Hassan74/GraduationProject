using System;

namespace SmartMicrobus.Core.DTO.Microbus
{
    public class MicrobusResponse
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = null!;
        public string? QrCode { get; set; }
        public bool IsActive { get; set; }
        public Guid RouteId { get; set; }
        public Guid? DriverId { get; set; }
        public int PassengerCount { get; set; }
        public string Model { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}
