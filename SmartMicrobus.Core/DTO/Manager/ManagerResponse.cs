namespace SmartMicrobus.Core.DTO.Manager
{
    public class ManagerResponse
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
    }
}
