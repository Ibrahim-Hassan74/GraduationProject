namespace SmartMicrobus.Core.Domain.Entities
{
    public class FavoriteRoute : BaseEntity<Guid>
    {
        public Guid PassengerId { get; set; }
        public Passenger Passenger { get; set; } = null!;

        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
