using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<LocationTrackingService> _localizer;

        private const string CacheKeyPrefix = "driver_location_";

        public LocationTrackingService(
            IMemoryCache cache,
            IUnitOfWork unitOfWork,
            IOsrmRouteService osrmRouteService,
            ILocationBroadcastService locationBroadcastService,
            IStringLocalizer<LocationTrackingService> localizer)
        {
            _cache = cache;
            _tripRepository = unitOfWork.TripRepository;
            _osrmRouteService = osrmRouteService;
            _locationBroadcastService = locationBroadcastService;
            _localizer = localizer;
        }

       
        public async Task<ApiResponse> UpdateDriverLocationAsync(Guid driverId, double latitude, double longitude)
        {
           
            if (latitude < -90 || latitude > 90)
                return ApiResponseFactory.BadRequest(_localizer["InvalidLatitude"]);

            if (longitude < -180 || longitude > 180)
                return ApiResponseFactory.BadRequest(_localizer["InvalidLongitude"]);

            var location = new DriverLocationResponse
            {
                DriverId = driverId,
                Latitude = latitude,
                Longitude = longitude,
                LastUpdated = DateTimeOffset.UtcNow
            };

           
            if (_cache.TryGetValue(CacheKeyPrefix + driverId, out DriverLocationResponse? oldLocation))
            {
                var distance = CalculateDistanceInMeters(
                    oldLocation.Latitude, oldLocation.Longitude,
                    location.Latitude, location.Longitude);

                if (distance < 30)
                    return ApiResponseFactory.Success(_localizer["LocationIgnored"]);
            }

           
            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            };

            _cache.Set(CacheKeyPrefix + driverId, location, cacheOptions);

            var trip = await _tripRepository.GetTripByDriverIdAsync(driverId);

            if (trip == null)
                return ApiResponseFactory.NotFound(_localizer["TripNotFound"]);

           
            var route = await _osrmRouteService.GetRouteAsync(new RouteRequest
            {
                StartLat = latitude,
                StartLng = longitude,
                EndLat = trip.EndLat ?? 0.0,
                EndLng = trip.EndLng ?? 0.0,
            });

            var routeDTO = new RouteResultDTO
            {
                DriverId = driverId,
                Distance = route.Distance,
                Duration = route.Duration,
                Coordinates = route.Coordinates,
                LastUpdated = location.LastUpdated
            };

          
            await _locationBroadcastService.BroadcastDriverLocationAsync(driverId, routeDTO);

            return ApiResponseFactory.Success(_localizer["LocationUpdated"]);
        }

       
        public async Task<ApiResponse> GetDriverLocationAsync(Guid driverId)
        {
            var trip = await _tripRepository.GetTripByDriverIdAsync(driverId);

            if (trip == null)
                return ApiResponseFactory.NotFound(_localizer["TripNotFound"]);

            var latitude = trip.StartLat ?? 0.0;
            var longitude = trip.StartLng ?? 0.0;
            DateTimeOffset? lastUpdated = trip.StartedAt;

            
            if (_cache.TryGetValue(CacheKeyPrefix + driverId, out DriverLocationResponse? location))
            {
                latitude = location?.Latitude ?? latitude;
                longitude = location?.Longitude ?? longitude;
                lastUpdated = location?.LastUpdated;
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
                DriverId = driverId,
                Distance = route.Distance,
                Duration = route.Duration,
                Coordinates = route.Coordinates,
                LastUpdated = lastUpdated
            };

            return ApiResponseFactory.Success(_localizer["LocationRetrieved"], routeDTO);
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

        private static double ToRadians(double deg)
            => deg * (Math.PI / 180);
    }
}