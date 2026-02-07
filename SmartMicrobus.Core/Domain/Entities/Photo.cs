using SmartMicrobus.Core.Domain.IdentityEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Photo : BaseEntity<Guid>
    {
        public string ImageName { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }
    }
}
