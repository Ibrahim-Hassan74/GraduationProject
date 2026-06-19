using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Route;
using System.Globalization;
using RouteEntity = SmartMicrobus.Core.Domain.Entities.Route;

namespace SmartMicrobus.Core.Services.Route
{
    public class RoutesService : IRoutesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IMapper _mapper;
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IStringLocalizer<RoutesService> _localizer;
        private readonly IMemoryCache _cache;
        private readonly IOsrmRouteService _osrmRouteService;

        private const string CacheKeyPrefix = "driver_location_";

        public RoutesService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IStringLocalizer<RoutesService> localizer,
            IMemoryCache cache,
            IOsrmRouteService osrmRouteService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _routeRepository = _unitOfWork.RouteRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _tripRepository = _unitOfWork.TripRepository;
            _localizer = localizer;
            _cache = cache;
            _osrmRouteService = osrmRouteService;
        }

        public async Task<ApiResponse> AddRouteAsync(RouteAddRequest routeAddRequest)
        {
            if (routeAddRequest is null)
            {
                return ApiResponseFactory.Failure("Route data is required.", 404);
            }
            await _routeRepository.AddAsync(_mapper.Map<RouteEntity>(routeAddRequest));
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Route added successfully.");
        }

        public async Task<ApiResponse> UpdateRouteAsync(RouteUpdateRequest routeUpdateRequest)
        {
            if (routeUpdateRequest is null)
            {
                return ApiResponseFactory.Failure("Route data is required.", 404);
            }

            var route = await _routeRepository.GetByIdAsync(routeUpdateRequest.RouteId);
            if (route == null)
            {
                return ApiResponseFactory.Failure("Route not found.", 404);
            }

            _mapper.Map(routeUpdateRequest, route);
            await _routeRepository.UpdateAsync(route);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Route updated successfully.");
        }
        public async Task<ApiResponse> GetAllRoutesAsync()
        {
            var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

            var cities = await _routeRepository.GetDistinctFromCitiesAsync(isArabic);

            if (cities == null || !cities.Any())
                return ApiResponseFactory.Success(_localizer["NoRoutesFound"], new List<RouteLocationResponse>());

            return ApiResponseFactory.Success(_localizer["RoutesRetrievedSuccessfully"], cities);
        }

        public async Task<ApiResponse> GetPaginatedRoutesAsync(RouteQuery query, Guid stationId)
        {
            var (routes, totalCount) = await _routeRepository.GetPaginatedByStationAsync(stationId, query);

            var mappedRoutes = _mapper.Map<List<RouteDetails>>(routes);

            var result = new Pagination<List<RouteDetails>>(query.PageNumber, query.PageSize, totalCount,  mappedRoutes);

            return ApiResponseFactory.Success("Paginated routes retrieved successfully.", result);
        }
        //public async Task<ApiResponse> GetDestinationsByFromAsync(string from)
        //{
        //    if (string.IsNullOrWhiteSpace(from))
        //        return ApiResponseFactory.BadRequest(_localizer["FromValueRequired"]);

        //    var routes = await _routeRepository.GetRoutesByFromAsync(from);

        //    if (!routes.Any())
        //        return ApiResponseFactory.Success(_localizer["NoDestinationsFound"], new List<DestinationResponse>());

        //    var result = _mapper.Map<List<DestinationResponse>>(routes);

        //    return ApiResponseFactory.Success(_localizer["DestinationsRetrieved"], result);
        //}

        public async Task<ApiResponse> GetDestinationsByFromAsync(Guid fromStationId)
        {
            var routes = await _routeRepository.GetRoutesByFromAsync(fromStationId);

            if (!routes.Any())
                return ApiResponseFactory.Success(_localizer["NoDestinationsFound"],new List<DestinationResponse>()
                );

            var result = _mapper.Map<List<DestinationResponse>>(routes);

            return ApiResponseFactory.Success(_localizer["DestinationsRetrieved"],result);
        }

        public async Task<ApiResponse> GetMicrobusesAtStationAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest(_localizer["RouteIdRequired"]);

            var queues = await _queueItemRepository.GetMicrobusesAtStationAsync(routeId);

            if (!queues.Any())
                return ApiResponseFactory.Success(_localizer["NoMicrobusesAtStation"], new List<MicrobusAtStationResponse>());

            var responses = _mapper.Map<List<MicrobusAtStationResponse>>(queues);

            return ApiResponseFactory.Success(_localizer["MicrobusesAtStationRetrieved"], responses);
        }

        public async Task<ApiResponse> GetMicrobusesOnTheWayAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest(_localizer["RouteIdRequired"]);

            var trips = await _tripRepository.GetMicrobusesOnTheWayAsync(routeId);

            if (trips == null || !trips.Any())
                return ApiResponseFactory.Success(_localizer["NoMicrobusesOnWay"], new List<MicrobusOnTheWayResponse>());

            var responses = _mapper.Map<List<MicrobusOnTheWayResponse>>(trips);

            return ApiResponseFactory.Success(_localizer["MicrobusesOnTheWayRetrieved"], responses);
        }

        public async Task<ApiResponse> GetRouteSearchResultAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest(_localizer["RouteIdRequired"]);

            var route = await _routeRepository.GetByIdAsync(routeId, r => r.FromStation, r => r.ToStation);

            if (route == null)
                return ApiResponseFactory.Success(_localizer["NoRoutesFound"], new RouteSummaryResponse());

            var onTheWay = await _tripRepository.GetMicrobusesOnTheWayCountAsync(routeId);
            var onQueues = await _queueItemRepository.GetMicrobusesAtStationCountAsync(routeId);
            var nearestArrival = await CalculateNearestArrivalOptimizedAsync(routeId, route);

            var summary = new RouteSummaryResponse
            {
                Price = route.Price,
                DistanceKm = route.DistanceKm,
                NumberOfMicrobusesInQueue = onQueues,
                NumberOfMicrobusesOnTheWay = onTheWay,
                NearestArrivalMinutes = nearestArrival >= 0 ? nearestArrival : null
            };

            return ApiResponseFactory.Success(_localizer["RouteSummaryRetrieved"], summary);
        }

        public async Task<ApiResponse> DeleteRouteAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
            {
                return ApiResponseFactory.Failure("RouteId is required.", 404);
            }

            await _routeRepository.DeleteAsync(routeId);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Route deleted successfully.");
        }

        public async Task<ApiResponse> GetRouteByIdAsync(Guid routeId, Guid stationId)
        {
            var result = await _routeRepository.GetByIdAsync(routeId);

            if (result == null)
                return ApiResponseFactory.NotFound("Route not found.");

            if(stationId != result.FromStationId)
                return ApiResponseFactory.Unauthorized("You are not authorized to access this route.");

            var response = _mapper.Map<RouteDetails>(result);

            return ApiResponseFactory.Success("Route retrieved successfully.", response);
        }

        private async Task<int> CalculateNearestArrivalOptimizedAsync(Guid routeId, RouteEntity route)
        {
            // 1. If microbuses are already at the station, arrival = 0
            var queueCount = await _queueItemRepository.GetMicrobusesAtStationCountAsync(routeId);
            if (queueCount > 0) return 0;

            var activeTrips = await _tripRepository.GetMicrobusesOnTheWayAsync(routeId);
            if (!activeTrips.Any()) return -1;

            var destLat = route.ToStation.Latitude;
            var destLng = route.ToStation.Longitude;

            // 2. Score each trip by straight-line distance to destination
            var scored = activeTrips.Select(trip =>
            {
                double lat, lng;
                if (_cache.TryGetValue(CacheKeyPrefix + trip.DriverId, out DriverLocationResponse? loc))
                {
                    lat = loc.Latitude;
                    lng = loc.Longitude;
                }
                else
                {
                    lat = trip.StartLat ?? 0;
                    lng = trip.StartLng ?? 0;
                }

                var distance = CalculateDistanceInMeters(lat, lng, destLat, destLng);
                return (Trip: trip, Lat: lat, Lng: lng, Distance: distance);
            })
            .OrderBy(x => x.Distance)
            .Take(3) // Top-3 closest by spatial distance
            .ToList();

            // 3. OSRM only for the top-3 candidates (parallel)
            const int TurnaroundMinutes = 5;

            // Cache the return trip duration — it's the same for all (ToStation → FromStation)
            var returnTrip = await _osrmRouteService.GetRouteAsync(new RouteRequest
            {
                StartLat = destLat,
                StartLng = destLng,
                EndLat = route.FromStation.Latitude,
                EndLng = route.FromStation.Longitude
            });

            var osrmTasks = scored.Select(async s =>
            {
                var result = await _osrmRouteService.GetRouteAsync(new RouteRequest
                {
                    StartLat = s.Lat,
                    StartLng = s.Lng,
                    EndLat = destLat,
                    EndLng = destLng
                });
                return result.Duration + returnTrip.Duration + TurnaroundMinutes;
            });

            var etas = await Task.WhenAll(osrmTasks);

            return (int)Math.Ceiling(etas.Min());
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