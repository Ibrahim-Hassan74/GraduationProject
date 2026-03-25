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

namespace SmartMicrobus.Core.Services.Drivers
{
    public class TripService(IQueueItemRepository _queueItemRepository, ITripRepository _tripRepository,
        IUnitOfWork _unitOfWork, IMapper _mapper) : ITripService
    {

        private readonly IStringLocalizer<TripService> _localizer;
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
