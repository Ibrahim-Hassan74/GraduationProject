using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessageResourceName = "RequiredPhoneNumber",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [EgyptianPhone(ErrorMessageResourceName = "InvalidPhoneNumber",
                       ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string PhoneNumber { get; set; } = null!;
    }
}
