namespace SmartMicrobus.Core.DTO.Route
{
    public class DestinationResponse
    {
        public Guid RouteId { get; set; }

        public string? To { get; set; }

        public Guid StationId { get; set; }
    }
}
