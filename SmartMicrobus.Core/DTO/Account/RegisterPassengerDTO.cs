using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class RegisterPassengerDTO
    {
        [Required(ErrorMessageResourceName = "RequiredDisplayName",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "RequiredPhoneNumber",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [EgyptianPhone(ErrorMessageResourceName = "InvalidPhoneNumber",
                       ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "RequiredPassword",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLengthPassword",
                      ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string Password { get; set; } = string.Empty;
    }
}