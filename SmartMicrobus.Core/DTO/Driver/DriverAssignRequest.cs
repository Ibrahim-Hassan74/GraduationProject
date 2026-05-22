using System;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverAssignRequest
    {
        public Guid DriverId { get; set; }
        public Guid MicrobusId { get; set; }
    }
}
