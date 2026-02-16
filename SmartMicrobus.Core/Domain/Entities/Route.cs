
namespace SmartMicrobus.Core.Domain.Entities
{
    public class Route
    {
        public Guid Id { get; set; }

        public string From { get; set; } = null!;
        public string To { get; set; } = null!;

        public decimal Price { get; set; }

        public Guid StationId { get; set; }
        public Station Station { get; set; } = null!;
    }

}
