namespace SmartMicrobus.Core.DTO.Route
{
    public class FavoriteRouteResponse
    {
        public Guid Id { get; set; }

        public Guid RouteId { get; set; }

        public string From { get; set; } = null!;

        public string To { get; set; } = null!;

        public decimal Price { get; set; }
    }
}
