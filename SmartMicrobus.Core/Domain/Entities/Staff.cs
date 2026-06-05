using SmartMicrobus.Core.Domain.IdentityEntities;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Staff : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public Guid StationId { get; set; }
        public Station Station { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
