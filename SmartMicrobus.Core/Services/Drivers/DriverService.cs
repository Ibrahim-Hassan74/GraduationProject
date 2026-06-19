using AutoMapper;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.DTO.Trip;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.Resources;
using SmartMicrobus.Core.Resources.Services.Drivers;
using SmartMicrobus.Core.ServiceContracts.Drivers;
using SmartMicrobus.Core.ServiceContracts.Route;

namespace SmartMicrobus.Core.Services.Drivers
{
    public class DriverService : IDriverService
    {
        private readonly IMapper _mapper;
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<DriverService> _localizer;
        private readonly IDriverRepository _driverRepository;
        private readonly IRouteTrackingNotificationService _routeTrackingNotificationService;
        public DriverService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IStringLocalizer<DriverService> localizer,
            IRouteTrackingNotificationService routeTrackingNotificationService)
        {
            _queueItemRepository = unitOfWork.QueueItemRepository;
            _tripRepository = unitOfWork.TripRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _driverRepository = unitOfWork.DriverRepository;
            _routeTrackingNotificationService = routeTrackingNotificationService;
        }


            if (trip == null)
                return ApiResponseFactory.Failure(_localizer["NoActiveTripFound"], 404);

            trip.Status = TripStatus.Completed;
            trip.EndedAt = DateTimeOffset.UtcNow;

            await _tripRepository.UpdateAsync(trip);
            await _unitOfWork.CompleteAsync();

            // Clean up driver's ETA and notify route subscribers
            _routeTrackingNotificationService.RemoveDriverEta(trip.RouteId, driverId);
            await _routeTrackingNotificationService.NotifyRouteUpdated(trip.RouteId);

            return ApiResponseFactory.Success(_localizer["TripEndedSuccessfully"]);
        }
        public async Task<ApiResponseWithData<DriverDashboardDTO>> GetDashboardAsync(Guid driverId)
        {
            var activeTrip = await _tripRepository.GetActiveTripAsync(driverId);

            if (activeTrip != null)
            {
                var tripDto = _mapper.Map<TripDashboardDTO>(activeTrip);

                var dashboard = new DriverDashboardDTO
                {
                    DriverId = driverId,
                    Status = DriverDashboardStatus.OnTrip.ToString(),
                    Trip = tripDto
                };

                return ApiResponseFactory.Success(_localizer["DriverOnTrip"], dashboard);
            }

            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
            {
                return ApiResponseFactory.Success(_localizer["DriverAvailable"],
                    new DriverDashboardDTO
                    {
                        DriverId = driverId,
                        Status = DriverDashboardStatus.Available.ToString()
                    });
            }

            var queueDto = _mapper.Map<QueueDashboardDTO>(queueItem);

            queueDto.DriversBefore =
                await _queueItemRepository.CountDriversBeforeAsync(queueItem.QueueId, queueItem.Position);

            queueDto.TotalDrivers =
                await _queueItemRepository.CountActiveAsync(queueItem.QueueId);

            var dashboardResult = new DriverDashboardDTO
            {
                DriverId = driverId,
                Status = DriverDashboardStatus.InQueue.ToString(),
                Queue = queueDto
            };

            return ApiResponseFactory.Success(_localizer["DriverInQueue"], dashboardResult);
        }


        public async Task<ApiResponseWithData<int>> GetDriversBeforeMeAsync(Guid driverId)
        {
            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
                return ApiResponseFactory.NotFound<int>(_localizer["DriverNotInQueue"]);

            var count = await _queueItemRepository
                .CountDriversBeforeAsync(queueItem.QueueId, queueItem.Position);

            return ApiResponseFactory.Success(_localizer["DriversCountRetrieved"], count);
        }
        public async Task<ApiResponseWithData<IEnumerable<QueueItemResponse>>> GetMyQueueAsync(Guid driverId)
        {
            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
                return ApiResponseFactory.NotFound<IEnumerable<QueueItemResponse>>(_localizer["DriverNotInQueue"]);

            var items = await _queueItemRepository.GetActiveQueueItemsAsync(queueItem.QueueId);

            var result = _mapper.Map<List<QueueItemResponse>>(items);

            return ApiResponseFactory.Success<IEnumerable<QueueItemResponse>>(_localizer["QueueRetrievedSuccessfully"], result);
        }

        public async Task ResetDailyQueueAsync()
        {
            var today = DateTimeOffset.UtcNow.Date;
            var now = DateTimeOffset.UtcNow;

            var pendingItems = await _queueItemRepository
                .GetAllActiveBeforeDateAsync(today);

            if (!pendingItems.Any())
                return;

            var groupedByQueue = pendingItems
                .GroupBy(x => x.QueueId);

            foreach (var group in groupedByQueue)
            {
                var queueId = group.Key;

                int newPosition = 1;

                var orderedItems = group
                    .OrderBy(x => x.Position)
                    .ToList();

                foreach (var item in orderedItems)
                {
                    item.Status = QueueStatus.Skipped;
                    item.LeftAt = now;

                    var newItem = new QueueItem
                    {
                        QueueId = queueId,
                        DriverId = item.DriverId,
                        MicrobusId = item.MicrobusId,
                        Position = newPosition,
                        Status = QueueStatus.Waiting,
                        JoinedAt = now
                    };

                    await _queueItemRepository.AddAsync(newItem);

                    newPosition++;
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<ApiResponse> StartTripAsync(Guid driverId)
        {
            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
                return ApiResponseFactory.NotFound(_localizer["DriverNotInQueue"]);

            var first = await _queueItemRepository.GetFirstInQueueAsync(queueItem.QueueId);

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

        public async Task<ApiResponseWithData<DriverResponse>> GetDriverByLicenseAsync(string licenseNumber)
            {
            var driver = await _driverRepository.GetDriverByLicense(licenseNumber);
            if (driver == null)
                return ApiResponseFactory.NotFound<DriverResponse>(_localizer["Driver_Not_Found_By_License"]);

            var driverInfo = _mapper.Map<DriverResponse>(driver);
            return ApiResponseFactory.Success(_localizer["Driver_Fetch_Success"], driverInfo);
        }

        public async Task<ApiResponseWithData<DriverResponse>> GetDriverByIdAsync(Guid driverId)
        {
            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver == null)
                return ApiResponseFactory.NotFound<DriverResponse>(_localizer["Driver_Not_Found_By_Id"]);

            var driverInfo = _mapper.Map<DriverResponse>(driver);
            return ApiResponseFactory.Success(_localizer["Driver_Fetch_Success"], driverInfo);
        }
    }
}