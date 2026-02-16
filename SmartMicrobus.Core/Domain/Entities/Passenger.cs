using SmartMicrobus.Core.Domain.IdentityEntities;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Passenger : BaseEntity<Guid>
    {
        public ApplicationUser ApplicationUser { get; set; }
    }
}
