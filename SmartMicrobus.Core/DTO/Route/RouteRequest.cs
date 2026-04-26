using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteRequest
    {
        public double StartLat { get; set; }
        public double StartLng { get; set; }

        public double EndLat { get; set; }
        public double EndLng { get; set; }

        public OverviewType Overview { get; set; } = OverviewType.Full;
        public GeometryType Geometry { get; set; } = GeometryType.GeoJson;
    }
}
