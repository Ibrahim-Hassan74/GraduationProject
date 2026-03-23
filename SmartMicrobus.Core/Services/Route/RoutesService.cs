using AutoMapper;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Route;
using System.Globalization;

namespace SmartMicrobus.Core.Services.Route
{
    public class RoutesService : IRoutesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IMapper _mapper;
        private readonly IQueueItemRepository _queueItemRepository;
        public RoutesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _routeRepository = _unitOfWork.RouteRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _tripRepository = _unitOfWork.TripRepository;
        }
        public async Task<ApiResponse> GetAllRoutesAsync()
        {
            var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

            var cities = await _routeRepository.GetDistinctFromCitiesAsync(isArabic);

            if (cities == null || !cities.Any())
                return ApiResponseFactory.Success("No routes found.", new List<RouteLocationResponse>());

            var result = cities.Select(c => new RouteLocationResponse
            {
                CityName = c
            }).ToList();

            return ApiResponseFactory.Success("Routes retrieved successfully", result);
        }

        public async Task<ApiResponse> GetDestinationsByFromAsync(string from)
        {
            if (string.IsNullOrWhiteSpace(from))
                return ApiResponseFactory.BadRequest("From value is required.");

            var routes = await _routeRepository.GetRoutesByFromAsync(from);

            if (!routes.Any())
                return ApiResponseFactory.Success("No destinations found for the specified origin.", new List<DestinationResponse>());

            var result = _mapper.Map<List<DestinationResponse>>(routes);

            return ApiResponseFactory.Success("Destinations retrieved.", result);
        }

        public async Task<ApiResponse> GetMicrobusesAtStationAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("RouteId is required.");

            var queues = await _queueItemRepository.GetMicrobusesAtStationAsync(routeId);

            if (!queues.Any())
                return ApiResponseFactory.Success("No microbuses at station for the specified route.", new List<MicrobusAtStationResponse>());

            var responses = _mapper.Map<List<MicrobusAtStationResponse>>(queues);

            return ApiResponseFactory.Success("Microbuses at station retrieved.", responses);
        }


        public async Task<ApiResponse> GetMicrobusesOnTheWayAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("RouteId is required.");

            var trips = await _tripRepository.GetMicrobusesOnTheWayAsync(routeId);

            if (trips == null || !trips.Any())
                return ApiResponseFactory.Success("No microbuses on the way for the specified route.", new List<MicrobusOnTheWayResponse>());

            var responses = _mapper.Map<List<MicrobusOnTheWayResponse>>(trips);

            return ApiResponseFactory.Success("Microbuses on the way retrieved.", responses);
        }


        public async Task<ApiResponse> GetRouteSearchResultAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("RouteId is required.");

            var route = await _routeRepository.GetByIdAsync(routeId);

            if (route == null)
                return ApiResponseFactory.Success("Route not found.", new RouteSummaryResponse());

            var onTheWay = await _tripRepository.GetMicrobusesOnTheWayCountAsync(routeId);

            var onQueues = await _queueItemRepository.GetMicrobusesAtStationCountAsync(routeId);



            var summary = new RouteSummaryResponse
            {
                Price = route.Price,
                DistanceKm = route.DistanceKm,
                NumberOfMicrobusesInQueue = onQueues,
                NumberOfMicrobusesOnTheWay = onTheWay,
                NearestArrivalMinutes = 0
            };

            return ApiResponseFactory.Success("Route summary retrieved.", summary);
        }
    }
}
