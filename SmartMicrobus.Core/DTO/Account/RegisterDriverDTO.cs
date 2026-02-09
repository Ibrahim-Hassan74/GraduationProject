using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class RegisterDriverDTO
    {
        public string DisplayName { get; set; } = string.Empty;
        [EgyptianPhone]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
    }
}
