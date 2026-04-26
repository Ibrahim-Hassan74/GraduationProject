using System.Text.Json;

namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteResult
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public List<List<double>> Coordinates { get; set; }
    }
    public class OsrmResponse
    {
        public List<RouteDTO> Routes { get; set; }
    }

    public class RouteDTO
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public JsonElement Geometry { get; set; }
    }

    public class GeometryDTO
    {
        public JsonElement Coordinates { get; set; }
    }
}
