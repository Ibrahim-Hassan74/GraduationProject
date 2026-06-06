using AutoMapper;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Driver;
using SmartMicrobus.Core.ServiceContracts.Route;
using SmartMicrobus.Core.Services.Common;

namespace SmartMicrobus.Core.Services.Drivers
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IStringLocalizer<TripService> _localizer;
        private readonly DriverDashboardRealtimeService _driverDashboardRealtime;
        private readonly IRouteTrackingNotificationService _routeTrackingNotificationService;

        public TripService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DriverDashboardRealtimeService driverDashboardRealtime,
            IStringLocalizer<TripService> localizer,
            IRouteTrackingNotificationService routeTrackingNotificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _tripRepository = _unitOfWork.TripRepository;
            _localizer = localizer;
            _driverDashboardRealtime = driverDashboardRealtime;
            _routeTrackingNotificationService = routeTrackingNotificationService;
        }
        public async Task<ApiResponse> StartTripAsync(Guid driverId)
        {
            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
                return ApiResponseFactory.NotFound(_localizer["DriverNotInQueue"]);

            var first = await _queueItemRepository
                .GetFirstInQueueAsync(queueItem.QueueId);

            if (first?.Id != queueItem.Id)
                return ApiResponseFactory.Conflict(_localizer["NotYourTurn"]);

            var trip = new Trip
            {
                DriverId = driverId,
                MicrobusId = queueItem.MicrobusId,
                RouteId = queueItem.Queue.RouteId,
                StartedAt = DateTimeOffset.UtcNow,
                Status = TripStatus.Started
            };

            await _tripRepository.AddAsync(trip);

            queueItem.Status = QueueStatus.InTrip;
            queueItem.LeftAt = DateTimeOffset.UtcNow;

            await _queueItemRepository.UpdateAsync(queueItem);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(_localizer["TripStartedSuccessfully"]);
        }

        public async Task<ApiResponse> EndTripAsync(Guid driverId)
        {
            var trip = await _tripRepository.GetActiveTripAsync(driverId);

            if (trip == null)
                return ApiResponseFactory.Failure(_localizer["NoActiveTripFound"], 404);

            trip.Status = TripStatus.Completed;
            trip.EndedAt = DateTimeOffset.UtcNow;

            await _tripRepository.UpdateAsync(trip);
            await _unitOfWork.CompleteAsync();
            await _driverDashboardRealtime.PushDashboard(driverId);

            // Clean up driver's ETA and notify route subscribers
            _routeTrackingNotificationService.RemoveDriverEta(trip.RouteId, driverId);
            await _routeTrackingNotificationService.NotifyRouteUpdated(trip.RouteId);

            return ApiResponseFactory.Success(_localizer["TripEndedSuccessfully"]);
        }

        public async Task<ApiResponse> GetDriverHistoryAsync(Guid driverId, DriverHistoryRequest request)
        {
            DateTime from;
            DateTime to;

            if (!request.FromDate.HasValue && !request.ToDate.HasValue)
            {
                from = DateTime.Today;
                to = DateTime.Today.AddDays(1);
            }
            else
            {
                from = request.FromDate ?? DateTime.MinValue;
                to = request.ToDate ?? DateTime.MaxValue;
            }

            var tripsHistory = await _tripRepository.GetDriverTripsAsync(driverId, request);

            if (tripsHistory == null || !tripsHistory.Trips.Any())
                return ApiResponseFactory.NotFound(_localizer["NoTripsFoundForPeriod"]);

            var history = _mapper.Map<List<TripHistoryDTO>>(tripsHistory.Trips);

            var response = new DriverHistoryResponse(
                tripsHistory.TotalAmount,
                history,
                tripsHistory.TotalCount);

            return ApiResponseFactory.Success(_localizer["DriverTripHistoryRetrieved"], response);
        }
    }
}
