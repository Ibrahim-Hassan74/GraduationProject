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
        public string? Address { get; set; }
        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<Queue> Queues { get; set; } = new List<Queue>();
    }
}
