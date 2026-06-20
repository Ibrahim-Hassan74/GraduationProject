using SmartMicrobus.Core.Helper;

namespace SmartMicrobus.Core.DTO.Staff
{
    public class StaffAddRequest
    {
        public string DisplayName { get; set; } = string.Empty;
        [EgyptianPhone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
