using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class VerifyOtpDTO
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number is invalid.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits.")]
        public string Otp { get; set; } = null!;
    }
}