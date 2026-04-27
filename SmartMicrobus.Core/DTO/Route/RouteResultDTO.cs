namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteResultDTO: RouteResult
    {
        public Guid DriverId { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
    }
}
