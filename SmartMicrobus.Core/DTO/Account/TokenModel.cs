using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class TokenModel
    {
        [Required(ErrorMessageResourceName = "RequiredToken",
          ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string? Token { get; set; } = string.Empty;
        [Required(ErrorMessageResourceName = "RequiredRefreshToken",
          ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
