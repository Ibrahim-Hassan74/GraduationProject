using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class ConfirmAccountDTO
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [EgyptianPhone]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits.")]
        public string Otp { get; set; } = null!;
    }
}
