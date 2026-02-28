using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Staff;

namespace SmartMicrobus.Core.Services.Staff
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMicrobusRepository _microbusRepository;
        private readonly IQueueRepository _queueRepository;
        private readonly IQueueItemRepository _queueItemRepository;

        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _microbusRepository = _unitOfWork.MicrobusRepository;
            _queueRepository = _unitOfWork.QueueRepository;
            _queueItemRepository = _unitOfWork.QueueItemRepository;

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
