using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number is invalid.")]
        public string PhoneNumber { get; set; } = null!;
    }
}
