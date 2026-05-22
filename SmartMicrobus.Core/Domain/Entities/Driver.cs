using SmartMicrobus.Core.Domain.IdentityEntities;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Driver : BaseEntity<Guid>
    {
        public string LicenseNumber { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public virtual Microbus? Microbus { get; set; }
    }
}
