namespace SmartMicrobus.Core.Domain.Entities
{
    public class Route : BaseEntity<Guid>
    {
        public string FromAr { get; set; } = null!;
        public string FromEn { get; set; } = null!;

        public string ToAr { get; set; } = null!;
        public string ToEn { get; set; } = null!;

        public decimal Price { get; set; }

        public Guid FromStationId { get; set; }
        public Station FromStation { get; set; } = null!;

        public Guid ToStationId { get; set; }
        public Station ToStation { get; set; } = null!;

        public double DistanceKm { get; set; }

        public ICollection<Trip> Trips { get; set; }
        public ICollection<Microbus> Microbuses { get; set; } = new List<Microbus>();
        public ICollection<Queue> Queues { get; set; } = new List<Queue>();
        public ICollection<FavoriteRoute> FavoriteRoutes { get; set; } = new List<FavoriteRoute>();
    }
}
