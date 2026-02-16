namespace SmartMicrobus.Core.Domain.Entities
{
    public class Route : BaseEntity<Guid>
    {
        public string FromAr { get; set; } = null!;
        public string FromEn { get; set; } = null!;

        public string ToAr { get; set; } = null!;
        public string ToEn { get; set; } = null!;

        public decimal Price { get; set; }

        public Guid StationId { get; set; }
        public Station Station { get; set; } = null!;

        public ICollection<Microbus> Microbuses { get; set; } = new List<Microbus>();
        public ICollection<Queue> Queues { get; set; } = new List<Queue>();
    }
}
