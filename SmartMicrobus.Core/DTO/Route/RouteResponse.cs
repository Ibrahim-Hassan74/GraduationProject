using System;

namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteResponse
    {
        public Guid Id { get; set; }
        public string FromAr { get; set; } = null!;
        public string FromEn { get; set; } = null!;
        public string ToAr { get; set; } = null!;
        public string ToEn { get; set; } = null!;
        public decimal Price { get; set; }
        public double DistanceKm { get; set; }
        public Guid FromStationId { get; set; }
        public Guid ToStationId { get; set; }
    }
}
