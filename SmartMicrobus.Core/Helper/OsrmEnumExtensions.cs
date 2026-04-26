using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.Helper
{
    public static class OsrmEnumExtensions
    {
        public static string ToQueryValue(this OverviewType overview)
        {
            return overview switch
            {
                OverviewType.Full => "full",
                OverviewType.Simplified => "simplified",
                OverviewType.False => "false",
                _ => "full"
            };
        }

        public static string ToQueryValue(this GeometryType geometry)
        {
            return geometry switch
            {
                GeometryType.GeoJson => "geojson",
                GeometryType.Polyline => "polyline",
                GeometryType.Polyline6 => "polyline6",
                _ => "geojson"
            };
        }
    }
}