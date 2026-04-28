namespace SmartMicrobus.Core.Helper
{
    public static class GeoValidator
    {
        public static bool IsValid(double lat, double lng)
        {
            return lat >= -90 && lat <= 90 &&
                   lng >= -180 && lng <= 180;
        }
    }
}
