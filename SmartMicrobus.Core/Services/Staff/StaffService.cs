using AutoMapper;
using Hangfire;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.ServiceContracts.Driver;
using SmartMicrobus.Core.ServiceContracts.Notification;
using SmartMicrobus.Core.ServiceContracts.Route;
using SmartMicrobus.Core.ServiceContracts.Staff;
using SmartMicrobus.Core.Services.Common;
using MyRoute = SmartMicrobus.Core.Domain.Entities.Route;

namespace SmartMicrobus.Core.Services.Staff
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMicrobusRepository _microbusRepository;
        private readonly IQueueRepository _queueRepository;
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IQueueNotificationService _queueNotificationService;
        private readonly IMapper _mapper;
        private readonly IQrTokenService _qrTokenService;
        private readonly ITripRepository _tripRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IStringLocalizer<StaffService> _localizer;
        private readonly DriverDashboardRealtimeService _dashboardRealtimeService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRouteTrackingNotificationService _routeTrackingNotificationService;

        public StaffService(IUnitOfWork unitOfWork,
            IQueueNotificationService queueNotificationService,
            IMapper mapper,
            IQrTokenService qrTokenService,
            IStringLocalizer<StaffService> localizer,
            DriverDashboardRealtimeService dashboardRealtimeService,
            IBackgroundJobClient backgroundJobClient,
            IRouteTrackingNotificationService routeTrackingNotificationService)
        {
            _unitOfWork = unitOfWork;
            _microbusRepository = _unitOfWork.MicrobusRepository;
            _queueRepository = _unitOfWork.QueueRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _queueNotificationService = queueNotificationService;
            _mapper = mapper;
            _qrTokenService = qrTokenService;
            _tripRepository = _unitOfWork.TripRepository;
            _routeRepository = _unitOfWork.RouteRepository;
            _localizer = localizer;
            _dashboardRealtimeService = dashboardRealtimeService;
            _backgroundJobClient = backgroundJobClient;
            _routeTrackingNotificationService = routeTrackingNotificationService;
        }

        public async Task<ApiResponse> CheckInAtGateAsync(string qrCode, Guid stationId)
        {
            // 0. Decode QR
            var payload = _qrTokenService.DecryptToken(qrCode);

            if (payload == null)
                return ApiResponseFactory.BadRequest(_localizer["Queue_Invalid_QR"]);

            // 1. Check if already in queue
            var existing = await _queueItemRepository.GetActiveByDriverIdAsync(payload.DriverId);

            if (existing != null)
                return ApiResponseFactory.BadRequest(_localizer["Queue_Already_In_Queue"]);

            // 2. Get base route
            var baseRoute = await _routeRepository.GetByIdAsync(payload.RouteId);

            if (baseRoute == null)
                return ApiResponseFactory.BadRequest(_localizer["InvalidRoute"]);

            // 3. Determine direction
            MyRoute? route = null;

            if (baseRoute.FromStationId == stationId)
            {
                route = baseRoute;
            }
            else if (baseRoute.ToStationId == stationId)
            {
                route = await _routeRepository.GetReverseRouteAsync(baseRoute);
            }

            if (route == null)
                return ApiResponseFactory.BadRequest(_localizer["MicrobusInvalidStartStation"]);

            // 4. Get Queue
            var queue = await _queueRepository.GetByStationAndRouteAsync(stationId, route.Id);

            if (queue == null)
                return ApiResponseFactory.BadRequest(_localizer["Queue_Not_Found"]);

            // 5. Get position
            var position = await _queueItemRepository.GetNextPositionAsync(queue.Id);

            #region 6. End active trip if exists
            var activeTrip = await _tripRepository.GetActiveTripAsync(payload.DriverId);

            if (activeTrip != null)
            {
                activeTrip.Status = TripStatus.Completed;
                activeTrip.EndedAt = DateTimeOffset.UtcNow;

                await _tripRepository.UpdateAsync(activeTrip);

                // Clean up driver's ETA from the route they just finished
                _routeTrackingNotificationService.RemoveDriverEta(activeTrip.RouteId, payload.DriverId);
            }
            #endregion

            // 7. Add QueueItem
            var item = new QueueItem
            {
                QueueId = queue.Id,
                DriverId = payload.DriverId,
                MicrobusId = payload.MicrobusId,
                Position = position,
                Status = QueueStatus.Waiting
            };

            await _queueItemRepository.AddAsync(item);
            await _unitOfWork.CompleteAsync();

            // 8. Push dashboard
            await _dashboardRealtimeService.PushDashboard(payload.DriverId);

            // 9. Reload item
            item = await _queueItemRepository.GetActiveByDriverIdAsync(item.DriverId);

            var queueResponse = _mapper.Map<QueueItemResponse>(item);

            // 10. Notify
            await _queueNotificationService.NotifyDriverAdded(queue.Id, queueResponse);

            // 11. Notify route subscribers (counts changed — bus arrived at station)
            await _routeTrackingNotificationService.NotifyRouteUpdated(route.Id);

            return ApiResponseFactory.Success(_localizer["Queue_Scan_Success"]);
        }

        public async Task<ApiResponse> CheckOutAtGateAsync(string qrCode)
        {
            // 0. Decode QR
            var payload = _qrTokenService.DecryptToken(qrCode);

            if (payload == null)
                return ApiResponseFactory.BadRequest(_localizer["Queue_Invalid_QR"]);

            // 1. Get active queue item
            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(payload.DriverId);

            if (queueItem == null)
                return ApiResponseFactory.BadRequest(_localizer["Queue_Driver_Not_In_Queue"]);

            // 2. Get microbus + base route
            var microbus = await _microbusRepository.GetByIdAsync(
                queueItem.MicrobusId,
                x => x.Route);

            if (microbus == null)
                return ApiResponseFactory.NotFound(_localizer["Queue_Microbus_Not_Found"]);

            var baseRoute = microbus.Route;

            if (baseRoute == null)
                return ApiResponseFactory.BadRequest(_localizer["RouteNotFound"]);

            // 3. Determine actual route based on station direction
            var startStationId = queueItem.Queue.StationId;

            MyRoute? route = null;

            if (baseRoute.FromStationId == startStationId)
            {
                route = baseRoute;
            }
            else if (baseRoute.ToStationId == startStationId)
            {
                route = await _routeRepository.GetReverseRouteAsync(baseRoute);
            }
            Console.WriteLine(
            $"Trip Route: {route.FromEn} -> {route.ToEn}"
            );
            if (route == null)
                return ApiResponseFactory.BadRequest(_localizer["InvalidStationForRoute"]);

            // 4. Stations
            var fromStation = await _unitOfWork.StationRepository.GetByIdAsync(route.FromStationId);
            var toStation = await _unitOfWork.StationRepository.GetByIdAsync(route.ToStationId);

            if (fromStation == null || toStation == null)
                return ApiResponseFactory.NotFound(_localizer["StationsNotFound"]);

            // 5. Create Trip
            var trip = new Trip
            {
                DriverId = payload.DriverId,
                MicrobusId = queueItem.MicrobusId,

                RouteId = route.Id,

                StationId = route.FromStationId,

                StartedAt = DateTimeOffset.UtcNow,
                Status = TripStatus.Started,

                PassengerCount = microbus.PassengerCount,
                TotalAmount = microbus.PassengerCount * route.Price,
                DistanceKm = route.DistanceKm,

                StartLat = fromStation.Latitude,
                StartLng = fromStation.Longitude,

                EndLat = toStation.Latitude,
                EndLng = toStation.Longitude,
            };

            await _tripRepository.AddAsync(trip);

            // 6. Update queue item
            queueItem.Status = QueueStatus.InTrip;
            queueItem.LeftAt = DateTimeOffset.UtcNow;

            await _queueItemRepository.UpdateAsync(queueItem);

            await _unitOfWork.CompleteAsync();

            // 7. Schedule auto trip ending
            const double MinBusSpeedKmPerHour = 30;

            var estimatedDuration =
                TimeSpan.FromHours(route.DistanceKm / MinBusSpeedKmPerHour);

            Console.WriteLine($"Distance: {route.DistanceKm}");
            Console.WriteLine($"ETA: {estimatedDuration}");
            Console.WriteLine($"Now: {DateTime.UtcNow}");
            Console.WriteLine($"Run At: {DateTime.UtcNow + estimatedDuration}");

            _backgroundJobClient.Schedule<ITripService>(
                x => x.EndTripAsync(payload.DriverId),
                estimatedDuration);

            // 8. Dashboard
            await _dashboardRealtimeService.PushDashboard(payload.DriverId);

            // 9. Queue realtime
            await _queueNotificationService.NotifyDriverRemoved(
                queueItem.QueueId,
                payload.DriverId);

            // 10. Route realtime
            await _routeTrackingNotificationService.NotifyRouteUpdated(route.Id);

            return ApiResponseFactory.Success(_localizer["Queue_Scan_Success"]);
        }
    }
}
