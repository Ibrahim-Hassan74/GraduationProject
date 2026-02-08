using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
