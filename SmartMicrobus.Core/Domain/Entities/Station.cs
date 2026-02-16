namespace SmartMicrobus.Core.Domain.Entities
{
    public class Station : BaseEntity<Guid>
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public string CityAr { get; set; } = null!;
        public string CityEn { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<Queue> Queues { get; set; } = new List<Queue>();
    }
}
