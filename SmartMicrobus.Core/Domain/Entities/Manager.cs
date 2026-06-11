using SmartMicrobus.Core.Domain.IdentityEntities;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Manager : BaseEntity<Guid>
    {
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
        public Guid StationId { get; set; }
        public virtual Station Station { get; set; } = null!;
    }
}
