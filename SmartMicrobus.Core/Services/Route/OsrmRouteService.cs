using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Route;
using System.Globalization;
using System.Text.Json;

namespace SmartMicrobus.Core.Services.Route
{
    public class OsrmRouteService : IOsrmRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<OsrmRouteService> _localizer;

        public OsrmRouteService(HttpClient httpClient, IConfiguration configuration, IStringLocalizer<OsrmRouteService> localizer)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _localizer = localizer;
        }

        public async Task<RouteResult> GetRouteAsync(RouteRequest request)
        {
            var url = _configuration["Osrm:BaseUrl"] +
                $"{request.TransportMode.ToQueryValue()}/" +
                $"{request.StartLng.ToString(CultureInfo.InvariantCulture)},{request.StartLat.ToString(CultureInfo.InvariantCulture)};{request.EndLng.ToString(CultureInfo.InvariantCulture)},{request.EndLat.ToString(CultureInfo.InvariantCulture)}" +
                $"?overview={request.Overview.ToQueryValue()}" +
                $"&geometries={request.Geometry.ToQueryValue()}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<OsrmResponse>(json, options);

            if (data?.Routes == null || !data.Routes.Any())
                throw new Exception(_localizer["NoRoutesFound"]);

            var route = data.Routes.First();

            List<List<double>> coordinates;

            if (request.Geometry == GeometryType.GeoJson)
            {
                var coords = route.Geometry.GetProperty("coordinates");

                coordinates = coords
                    .EnumerateArray()
                    .Select(p => p.EnumerateArray().Select(x => x.GetDouble()).ToList())
                    .ToList();
            }
            else
            {
                var encoded = route.Geometry.GetString();
                coordinates = DecodePolyline(encoded, request.Geometry == GeometryType.Polyline6);
            }

            return new RouteResult
            {
                Distance = route.Distance / 1000.00,
                Duration = route.Duration / 60.00,
                Coordinates = coordinates
            };
        }

        private List<List<double>> DecodePolyline(string encoded, bool isPolyline6)
        {
            var poly = new List<List<double>>();
            int index = 0, lat = 0, lng = 0;
            int factor = isPolyline6 ? 1000000 : 100000;

            while (index < encoded.Length)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;

                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                poly.Add(new List<double>
                {
                    lng / (double)factor,
                    lat / (double)factor
                });
            }

            return poly;
        }
    }

}
