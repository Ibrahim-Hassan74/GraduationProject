using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Drivers;

namespace SmartMicrobus.Core.Services.Drivers
{
    public class DriverService : IDriverService
    {
        private readonly IMapper _mapper;
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DriverService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _queueItemRepository = unitOfWork.QueueItemRepository;
            _tripRepository = unitOfWork.TripRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> EndTripAsync(Guid driverId)
        {
            var trip = await _tripRepository.GetActiveTripAsync(driverId);

            if (trip == null)
                return ApiResponseFactory.Failure("No active trip found for the driver", 404);

            trip.Status = TripStatus.Completed;
            trip.EndedAt = DateTimeOffset.UtcNow;

            await _tripRepository.UpdateAsync(trip);

            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Trip ended successfully");
        }

        public async Task<ApiResponseWithData<DriverDashboardDTO>> GetDashboardAsync(Guid driverId)
        {
            var activeTrip = await _tripRepository.GetActiveTripAsync(driverId);

            if (activeTrip != null)
            {
                return ApiResponseFactory.Success("Driver is currently on a trip", new DriverDashboardDTO
                {
                    Status = "InTrip",
                    RouteFrom = activeTrip.Route.FromAr,
                    RouteTo = activeTrip.Route.ToAr
                });
            }

            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
            {
                return ApiResponseFactory.Success("Driver is currently available", new DriverDashboardDTO
                {
                    Status = "Available"
                });
            }

            var dashboard = _mapper.Map<DriverDashboardDTO>(queueItem);

            dashboard.DriversBefore = await _queueItemRepository.CountDriversBeforeAsync(queueItem.QueueId, queueItem.Position);
            dashboard.TotalDrivers = await _queueItemRepository.CountActiveAsync(queueItem.QueueId);

            return ApiResponseFactory.Success("Driver is currently in the queue", dashboard);
        }

        public async Task<ApiResponseWithData<int>> GetDriversBeforeMeAsync(Guid driverId)
        {
            var queueItem = await _queueItemRepository
                .GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
                return ApiResponseFactory.NotFound<int>("Driver is not in queue.");

            var count = await _queueItemRepository
                .CountDriversBeforeAsync(queueItem.QueueId, queueItem.Position);

            return ApiResponseFactory.Success("Drivers count retrieved.", count);
        }

        public async Task<ApiResponseWithData<IEnumerable<QueueItemResponse>>> GetMyQueueAsync(Guid driverId)
        {
            var queueItem = await _queueItemRepository
                .GetActiveByDriverIdAsync(driverId);

            if (queueItem == null)
                return ApiResponseFactory.NotFound<IEnumerable<QueueItemResponse>>("Driver is not in any queue.");

            var items = await _queueItemRepository
                .GetActiveQueueItemsAsync(queueItem.QueueId);

            var result = _mapper.Map<List<QueueItemResponse>>(items);

            return ApiResponseFactory.Success<IEnumerable<QueueItemResponse>>("Queue retrieved successfully.", result);
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
                return ApiResponseFactory.NotFound("Driver not in queue.");

            var first = await _queueItemRepository
                .GetFirstInQueueAsync(queueItem.QueueId);

            if (first?.Id != queueItem.Id)
                return ApiResponseFactory.Conflict("It is not your turn.");

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

            return ApiResponseFactory.Success("Trip started successfully.");
        }

        public async Task<ApiResponseWithData<List<TripHistoryDTO>>> GetDriverHistoryAsync(Guid driverId, DateTime? fromDate, DateTime? toDate)
        {
            DateTime from;
            DateTime to;

            if (!fromDate.HasValue && !toDate.HasValue)
            {
                from = DateTime.Today;
                to = DateTime.Today.AddDays(1);
            }
            else
            {
                from = fromDate ?? DateTime.MinValue;
                to = toDate ?? DateTime.MaxValue;
            }

            var trips = await _tripRepository.GetDriverTripsAsync(driverId, from, to);

            if (trips == null || !trips.Any())
            {
                return ApiResponseFactory.Success("No trips found for the selected period", new List<TripHistoryDTO>());
            }

            var history = _mapper.Map<List<TripHistoryDTO>>(trips);

            return ApiResponseFactory.Success("Driver trip history retrieved successfully", history);
        }
    }
}