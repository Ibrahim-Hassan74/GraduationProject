using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class TokenModel
    {
        [Required(ErrorMessage = "{0} is required.")]
        public string? Token { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} is required.")]
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
