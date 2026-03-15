using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.DTO.Route
{
    public class MicrobusAtStationResponse : QueueItemResponse
    {
        public int PassengerCount { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}
