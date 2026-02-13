

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Queue
    {
        public Guid Id { get; set; }

        public Guid StationId { get; set; }
        public Station Station { get; set; } = null!;

        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public ICollection<QueueItem> Items { get; set; } = new List<QueueItem>();
    }

}
