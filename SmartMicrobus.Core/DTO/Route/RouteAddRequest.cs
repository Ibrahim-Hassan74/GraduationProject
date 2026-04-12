using SmartMicrobus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteAddRequest
    {
        public string FromAr { get; set; } = null!;
        public string FromEn { get; set; } = null!;

        public string ToAr { get; set; } = null!;
        public string ToEn { get; set; } = null!;

        public decimal Price { get; set; }

        public Guid StationId { get; set; }
        public Station Station { get; set; } = null!;

        public double DistanceKm { get; set; }
    }
}
