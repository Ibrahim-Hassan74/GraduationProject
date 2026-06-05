using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Enums;
using System;

namespace SmartMicrobus.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTimeOffset RefreshTokenExpirationDateTime { get; set; }
        public virtual Photo? Photo { get; set; }
        public virtual Staff? Staff { get; set; }
        //public UserRole Role { get; set; }
    }
}
