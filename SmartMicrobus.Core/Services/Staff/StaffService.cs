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

        public StaffService(IUnitOfWork unitOfWork, IQueueNotificationService queueNotificationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _microbusRepository = _unitOfWork.MicrobusRepository;
            _queueRepository = _unitOfWork.QueueRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;
            _queueNotificationService = queueNotificationService;
            _mapper = mapper;
        }


        public async Task<ApiResponse> CheckInAtGateAsync(string qrCode, Guid stationId)
        {
            var microbus = await _microbusRepository.GetByQrCodeAsync(qrCode);
            if (microbus == null)
                return ApiResponseFactory.NotFound("Microbus not found.");

            var queue = await GetOrCreateQueueAsync(microbus.RouteId, stationId);


            var lastPosition = await _queueItemRepository.GetLastPositionAsync(queue.Id);

            var existing = await _unitOfWork.QueueItemRepository.GetActiveByDriverIdAsync(microbus.DriverId);

            if (existing != null)
                return ApiResponseFactory.BadRequest("Microbus is in waiting queue. Use leave-waiting to start trip when its turn.");

            var item = new QueueItem
            {
                QueueId = queue.Id,
                DriverId = microbus.DriverId,
                MicrobusId = microbus.Id,
                Position = lastPosition + 1,
                Status = QueueStatus.Waiting
            };
            await _unitOfWork.QueueItemRepository.AddAsync(item);

            await _unitOfWork.CompleteAsync();

            item = await _unitOfWork.QueueItemRepository.GetActiveByDriverIdAsync(item.DriverId);

            var queueResponse = _mapper.Map<QueueItemResponse>(item);

            await _queueNotificationService.NotifyDriverAdded(queue.Id, queueResponse);

            return ApiResponseFactory.Success("Scan processed.");
        }

      
        public async Task<ApiResponse> CheckOutAtGateAsync(string qrCode)
        {
            var microbus = await _microbusRepository.GetByQrCodeAsync(qrCode);
            if (microbus == null)
                return ApiResponseFactory.NotFound("Microbus not found.");

            var queueItem = await _queueItemRepository.GetActiveByDriverIdAsync(microbus.DriverId);

            if (queueItem == null)
                return ApiResponseFactory.BadRequest("Driver not in queue");
             
            queueItem.Status = QueueStatus.Skipped;
            queueItem.LeftAt = DateTimeOffset.UtcNow;

            await _queueItemRepository.UpdateAsync(queueItem);

            await _unitOfWork.CompleteAsync();

            await _queueNotificationService.NotifyDriverRemoved(queueItem.QueueId, microbus.DriverId);

            return ApiResponseFactory.Success("Scan processed.");
        }

        private async Task<Domain.Entities.Queue> GetOrCreateQueueAsync(Guid routeId, Guid stationId)
        {
            var queue = await _queueRepository.GetByStationAndRouteAsync(stationId, routeId);
            if (queue != null) return queue;

            var newQueue = new Domain.Entities.Queue
            {
                RouteId = routeId,
                StationId = stationId
            };

            newQueue = await _queueRepository.AddAsync(newQueue);
            await _unitOfWork.CompleteAsync();

            return newQueue;
        }
    }
}
