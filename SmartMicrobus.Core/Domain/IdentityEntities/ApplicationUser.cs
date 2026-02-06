using Microsoft.AspNetCore.Identity;

namespace SmartMicrobus.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}
