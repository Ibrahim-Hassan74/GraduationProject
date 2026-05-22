using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessageResourceName = "RequiredPhoneNumber",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [EgyptianPhone(ErrorMessageResourceName = "InvalidPhoneNumber",
                       ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessageResourceName = "RequiredPassword",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLengthPassword",
                      ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}