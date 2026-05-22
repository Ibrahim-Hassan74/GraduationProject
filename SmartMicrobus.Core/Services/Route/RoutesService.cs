using AutoMapper;
using Microsoft.Extensions.Localization;
using RouteEntity = SmartMicrobus.Core.Domain.Entities.Route;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.Resources.Services.Route;
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
        private readonly IStringLocalizer<RoutesService> _localizer;

        public RoutesService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IStringLocalizer<RoutesService> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _routeRepository = _unitOfWork.RouteRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _tripRepository = _unitOfWork.TripRepository;
            _localizer = localizer;
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

            var route = await _routeRepository.GetByIdAsync(routeId);

            if (route == null)
                return ApiResponseFactory.Success(_localizer["NoRoutesFound"],new RouteSummaryResponse() );

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
    }
}