using Microsoft.Extensions.Caching.Memory;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Driver;
using SmartMicrobus.Core.ServiceContracts.Notification;
using SmartMicrobus.Core.ServiceContracts.Route;

namespace SmartMicrobus.Core.Services.Drivers
{
    public class LocationTrackingService : ILocationTrackingService
    {
        private readonly IMemoryCache _cache;
        private readonly ITripRepository _tripRepository;
        private readonly IOsrmRouteService _osrmRouteService;
        private readonly ILocationBroadcastService _locationBroadcastService;
        private const string CacheKeyPrefix = "driver_location_";

        public LocationTrackingService(IMemoryCache cache, IUnitOfWork unitOfWork, IOsrmRouteService osrmRouteService, ILocationBroadcastService locationBroadcastService)
        {
            _cache = cache;
            _tripRepository = unitOfWork.TripRepository;
            _osrmRouteService = osrmRouteService;
            _locationBroadcastService = locationBroadcastService;
        }

        public async Task<ApiResponse> UpdateDriverLocationAsync(Guid driverId, double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                return ApiResponseFactory.BadRequest("Invalid latitude. Must be between -90 and 90.");

            if (longitude < -180 || longitude > 180)
                return ApiResponseFactory.BadRequest("Invalid longitude. Must be between -180 and 180.");

            var location = new DriverLocationResponse
            {
                DriverId = driverId,
                Latitude = latitude,
                Longitude = longitude,
                LastUpdated = DateTimeOffset.UtcNow
            };

            if (_cache.TryGetValue(CacheKeyPrefix + driverId, out DriverLocationResponse? locationOut))
            {
                var distance = CalculateDistanceInMeters(locationOut.Latitude, locationOut.Longitude,
                    location.Latitude, location.Longitude
                );

                if (distance < 30)
                {
                    return ApiResponseFactory.Success("Location update ignored (movement أقل من 30 متر)");
                }
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            };

            _cache.Set(CacheKeyPrefix + driverId, location, cacheOptions);

            var trip = await _tripRepository.GetTripByDriverIdAsync(driverId);

            if (trip == null)
                return ApiResponseFactory.NotFound("No active trip found for this driver.");

            var route = await _osrmRouteService.GetRouteAsync(new RouteRequest
            {
                StartLat = latitude,
                StartLng = longitude,
                EndLat = trip.EndLat ?? 0.0,
                EndLng = trip.EndLng ?? 0.0,
            });

            var routeDTO = new RouteResultDTO
            {
                Distance = route.Distance,
                Duration = route.Duration,
                Coordinates = route.Coordinates,
                LastUpdated = location.LastUpdated,
                DriverId = driverId
            };
            await _locationBroadcastService.BroadcastDriverLocationAsync(driverId, routeDTO);

            return ApiResponseFactory.Success("Location updated successfully.");
        }

        public async Task<ApiResponse> GetDriverLocationAsync(Guid driverId)
        {
            var trip = await _tripRepository.GetTripByDriverIdAsync(driverId);
            if (trip == null)
                return ApiResponseFactory.NotFound("No active trip found for this driver.");
            var latitude = trip.StartLat ?? 0.0;
            var longitude = trip.StartLng ?? 0.0;

            if (_cache.TryGetValue(CacheKeyPrefix + driverId, out DriverLocationResponse? location))
            {
                latitude = location?.Latitude??latitude;
                longitude= location?.Longitude??longitude;
            }

            var route = await _osrmRouteService.GetRouteAsync(new RouteRequest
            {
                StartLat = latitude,
                StartLng = longitude,
                EndLat = trip.EndLat ?? 0.0,
                EndLng = trip.EndLng ?? 0.0,
            });

            var routeDTO = new RouteResultDTO
            {
                Distance = route.Distance,
                Duration = route.Duration,
                Coordinates = route.Coordinates,
                LastUpdated = location?.LastUpdated,
                DriverId = driverId
            };
            return ApiResponseFactory.Success("Location retrieved successfully.", routeDTO);

        }

        private static double CalculateDistanceInMeters(
                double lat1, double lon1,
                double lat2, double lon2)
        {
            const double R = 6371000;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRadians(double deg) => deg * (Math.PI / 180);

    }
}