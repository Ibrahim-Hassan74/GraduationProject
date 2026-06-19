namespace SmartMicrobus.Core.DTO.Admin
{
    public class UserListItem
    {
        public Guid Id { get; set; } 
        public string DisplayName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? PhotoName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
