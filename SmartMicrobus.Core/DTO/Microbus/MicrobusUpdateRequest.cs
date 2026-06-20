using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Core.DTO.Microbus
{
    public class MicrobusUpdateRequest
    {
        public string? PlateNumber { get; set; }
        public Guid? RouteId { get; set; }
        public int? PassengerCount { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public bool? IsActive { get; set; }
    }
}
