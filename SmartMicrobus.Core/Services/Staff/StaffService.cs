using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.ServiceContracts.Staff;

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

        public StaffService(IUnitOfWork unitOfWork,
            IQueueNotificationService queueNotificationService,
            IMapper mapper,
            IQrTokenService qrTokenService)
        {
            _unitOfWork = unitOfWork;
            _microbusRepository = _unitOfWork.MicrobusRepository;
            _queueRepository = _unitOfWork.QueueRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _queueNotificationService = queueNotificationService;
            _mapper = mapper;
            _qrTokenService = qrTokenService;
            _tripRepository = _unitOfWork.TripRepository;
        }

        public async Task<ApiResponse> CheckInAtGateAsync(string qrCode, Guid stationId)
        {
            var payload = _qrTokenService.DecryptToken(qrCode);

            if (payload == null)
                return ApiResponseFactory.BadRequest("Invalid or expired QR code.");

            var existing = await _queueItemRepository.GetActiveByDriverIdAsync(payload.DriverId);

            if (existing != null)
                return ApiResponseFactory.BadRequest("Microbus is already in queue.");

            var lastPosition = await _queueItemRepository.GetLastPositionAsync(payload.QueueId);

            #region end active trip if exists
            var activeTrip = await _tripRepository.GetActiveTripAsync(payload.DriverId);

            if (activeTrip != null)
            {
                activeTrip.Status = TripStatus.Completed;
                activeTrip.EndedAt = DateTimeOffset.UtcNow;

                await _tripRepository.UpdateAsync(activeTrip);
            }
            #endregion

            var item = new QueueItem
            {
                QueueId = payload.QueueId,
                DriverId = payload.DriverId,
                MicrobusId = payload.MicrobusId,
                Position = lastPosition + 1,
                Status = QueueStatus.Waiting
            };

            await _queueItemRepository.AddAsync(item);

            await _unitOfWork.CompleteAsync();

            item = await _queueItemRepository.GetActiveByDriverIdAsync(item.DriverId);

            var queueResponse = _mapper.Map<QueueItemResponse>(item);

            await _queueNotificationService.NotifyDriverAdded(payload.QueueId, queueResponse);

            return ApiResponseFactory.Success("Scan processed.");
        }

        public async Task<ApiResponse> CheckOutAtGateAsync(string qrCode)
        {
            var payload = _qrTokenService.DecryptToken(qrCode);

            if (payload == null)
                return ApiResponseFactory.BadRequest("Invalid or expired QR code.");

            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(payload.DriverId);

            if (queueItem == null)
                return ApiResponseFactory.BadRequest("Driver not in queue");

            //var first = await _queueItemRepository
            //    .GetFirstInQueueAsync(queueItem.QueueId);

            //if (first?.Id != queueItem.Id)
            //    return ApiResponseFactory.Conflict("It is not your turn.");

            var microbus = await _microbusRepository.GetByIdAsync(queueItem.MicrobusId, x => x.Route);

            if (microbus == null)
                return ApiResponseFactory.NotFound("Microbus not found.");

            var trip = new Trip
            {
                DriverId = payload.DriverId,
                MicrobusId = queueItem.MicrobusId,
                RouteId = microbus.RouteId,
                StartedAt = DateTimeOffset.UtcNow,
                Status = TripStatus.Started,
                PassengerCount = microbus.PassengerCount,
                TotalAmount = microbus.PassengerCount * microbus.Route.Price,
                DistanceKm = microbus.Route.DistanceKm
            };

            await _tripRepository.AddAsync(trip);

            queueItem.Status = QueueStatus.InTrip;

            queueItem.LeftAt = DateTimeOffset.UtcNow;

            await _queueItemRepository.UpdateAsync(queueItem);

            await _unitOfWork.CompleteAsync();

            await _queueNotificationService.NotifyDriverRemoved(queueItem.QueueId, payload.DriverId);

            return ApiResponseFactory.Success("Scan processed.");
        }
    }
}
