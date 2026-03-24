using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessageResourceName = "RequiredUserId",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public Guid UserId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredToken",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string Token { get; set; } = null!;

        [Required(ErrorMessageResourceName = "RequiredPassword",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLengthPassword",
                      ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessageResourceName = "RequiredConfirmPassword",
                  ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLengthPassword",
                      ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [Compare("NewPassword",
            ErrorMessageResourceName = "PasswordMismatch",
            ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string ConfirmPassword { get; set; } = null!;
    }
}