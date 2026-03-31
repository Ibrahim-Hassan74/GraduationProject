using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Driver;
using SmartMicrobus.Core.Services.Common;

namespace SmartMicrobus.Core.Services.Drivers
{
    public class TripService(IUnitOfWork _unitOfWork, IMapper _mapper, DriverDashboardRealtimeService driverDashboardRealtime) : ITripService
    {
        private readonly IQueueItemRepository _queueItemRepository = _unitOfWork.QueueItemRepository;
        private readonly ITripRepository _tripRepository = _unitOfWork.TripRepository;
        private readonly DriverDashboardRealtimeService _driverDashboardRealtime = driverDashboardRealtime;
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
        public async Task<ApiResponse> EndTripAsync(Guid driverId)
        {
            var trip = await _tripRepository.GetActiveTripAsync(driverId);

            if (trip == null)
                return ApiResponseFactory.Failure("No active trip found for the driver", 404);

            trip.Status = TripStatus.Completed;
            trip.EndedAt = DateTimeOffset.UtcNow;

            await _tripRepository.UpdateAsync(trip);

            await _unitOfWork.CompleteAsync();
            await _driverDashboardRealtime.PushDashboard(driverId); 

            return ApiResponseFactory.Success("Trip ended successfully");
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
            {
                return ApiResponseFactory.NotFound("No trips found for the selected period");
            }

            var history = _mapper.Map<List<TripHistoryDTO>>(tripsHistory.Trips);
            var response = new DriverHistoryResponse(tripsHistory.TotalAmount, history, tripsHistory.TotalCount);

            return ApiResponseFactory.Success("Driver trip history retrieved successfully", response);
        }
    }
}
