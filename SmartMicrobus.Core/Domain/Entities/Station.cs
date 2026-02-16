

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Station
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string City { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Route> Routes { get; set; } = new List<Route>();
    }

}
