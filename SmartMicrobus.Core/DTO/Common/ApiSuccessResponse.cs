namespace SmartMicrobus.Core.DTO.Common
{
    public class ApiSuccessResponse : ApiResponse
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Token { get; set; } = string.Empty;
        public DateTimeOffset? Expiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTimeOffset RefreshTokenExpirationDateTime { get; set; }
    }
}
