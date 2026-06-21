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
        public Guid ToStationId { get; set; }
        public decimal Price { get; set; }
        public double DistanceKm { get; set; }
    }
}
