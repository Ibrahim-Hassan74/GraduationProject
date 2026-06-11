using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Manager
{
    public class RegisterManagerDTO
    {
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [EgyptianPhone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public Guid StationId { get; set; }
    }
}
