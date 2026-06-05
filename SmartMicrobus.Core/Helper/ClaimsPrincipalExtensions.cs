using System.Security.Claims;


namespace SmartMicrobus.Core.Helper
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetStationId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(CustomClaims.StationId)?.Value;

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("StationId claim not found");

            return Guid.Parse(value);
        }
    }
    public static class CustomClaims
    {
        public const string StationId = "station_id";
    }
}
