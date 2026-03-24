using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class VerifyOtpDTO
    {
        [Required(ErrorMessageResourceName = "RequiredPhoneNumber",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [EgyptianPhone(ErrorMessageResourceName = "InvalidPhoneNumber",
                       ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessageResourceName = "RequiredOtp",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [RegularExpression(@"^\d{6}$",
            ErrorMessageResourceName = "InvalidOtp",
            ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string Otp { get; set; } = null!;
    }
}