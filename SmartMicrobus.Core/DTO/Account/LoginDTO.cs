using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number is invalid.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}
