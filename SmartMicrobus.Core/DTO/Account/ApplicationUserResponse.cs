namespace SmartMicrobus.Core.DTO.Account
{
    public class ApplicationUserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Roles { get; set; } 
        public string? PhotoUrl { get; set; }
    }
}
