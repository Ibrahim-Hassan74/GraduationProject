using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [EgyptianPhone]
        public string PhoneNumber { get; set; } = null!;
    }
}
