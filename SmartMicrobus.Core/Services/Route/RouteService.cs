using AutoMapper;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Route;
using System.Globalization;

namespace SmartMicrobus.Core.Services.Route
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IMapper _mapper;
        private readonly IQueueItemRepository _queueItemRepository;
        public RouteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _routeRepository = _unitOfWork.RouteRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _tripRepository = _unitOfWork.TripRepository;
        }
        public async Task<ApiResponse> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllAsync();

            if (routes == null || !routes.Any())
                return ApiResponseFactory.NotFound("No routes found.");
            var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? true : false;
            var result = routes.Select(r =>
                isArabic ? r.FromAr : r.FromEn
            ).Distinct().ToList();

            return ApiResponseFactory.Success("Routes retrieved successfully", result);
        }

        public async Task<ApiResponse> GetDestinationsByFromAsync(string from)
        {
            if (string.IsNullOrWhiteSpace(from))
                return ApiResponseFactory.BadRequest("From value is required.");

            var routes = await _routeRepository.GetRoutesByFromAsync(from);

            if (!routes.Any())
                return ApiResponseFactory.NotFound("No destinations found for the specified origin.");

            var result = _mapper.Map<DestinationResponse>(routes);

            return ApiResponseFactory.Success("Destinations retrieved.", result);
        }

        public async Task<ApiResponse> GetMicrobusesAtStationAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("RouteId is required.");

            var queues = await _queueItemRepository.GetMicrobusesAtStationAsync(routeId);

            if (!queues.Any())
                return ApiResponseFactory.NotFound("No microbuses at station for the specified route.");

            var responses = _mapper.Map<List<MicrobusAtStationResponse>>(queues);

            return ApiResponseFactory.Success("Microbuses at station retrieved.", responses);
        }


        public async Task<ApiResponse> GetMicrobusesOnTheWayAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("RouteId is required.");

            var trips = await _tripRepository.GetMicrobusesOnTheWayAsync(routeId);

            if (trips == null || !trips.Any())
                return ApiResponseFactory.NotFound("No microbuses on the way for the specified route.");

            var responses = _mapper.Map<List<MicrobusOnTheWayResponse>>(trips);

            return ApiResponseFactory.Success("Microbuses on the way retrieved.", responses);
        }

       
        public async Task<ApiResponse> GetRouteSearchResultAsync(Guid routeId)
        {
            if (routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("RouteId is required.");

            var route = await _routeRepository.GetByIdAsync(routeId);

            if (route == null)
                return ApiResponseFactory.NotFound("Route not found.");

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
