using NetTopologySuite.Geometries;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Station : BaseEntity<Guid>
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public string CityAr { get; set; } = null!;
        public string CityEn { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? AddressAr { get; set; }
        public string? AddressEn { get; set; }
        public Point? Location { get; set; } = null!;
        //public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<Route> FromRoutes { get; set; } = new List<Route>();
        public ICollection<Route> ToRoutes { get; set; } = new List<Route>();
        public ICollection<Queue> Queues { get; set; } = new List<Queue>();
    }
}
