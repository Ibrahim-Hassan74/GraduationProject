using SmartMicrobus.Core.Helper;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverAddRequest
    {
        public string LicenseNumber { get; set; }

        public string DriverName { get; set; }

        [EgyptianPhone]
        public string PhoneNumber { get; set; }
    }
}
