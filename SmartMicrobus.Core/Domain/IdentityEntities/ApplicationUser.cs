using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTimeOffset RefreshTokenExpirationDateTime { get; set; }
        public virtual Photo? Photo { get; set; }
    }
}
