namespace SmartMicrobus.Core.DTO.Staff
{
    public class StaffResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool HasPassword { get; set; }
    }
}
