using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteUpdateRequest
    {
        public Guid RouteId { get; set; }
        public string FromAr { get; set; } = null!;
        public string FromEn { get; set; } = null!;

        public string ToAr { get; set; } = null!;
        public string ToEn { get; set; } = null!;

        public decimal Price { get; set; }
        public double DistanceKm { get; set; }

    }
}
