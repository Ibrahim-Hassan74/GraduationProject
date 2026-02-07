using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class ResetPasswordDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Compare("NewPassword", ErrorMessage = "Passwords must match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
