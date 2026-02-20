using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Driver;

namespace SmartMicrobus.Core.Services.Driver
{
    public class DriverService : IDriverService
    {
        private readonly IMapper _mapper;
        private readonly IMicrobusRepository _microbusRepository;
        private readonly IQueueRepository _queueRepository;
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly ITripRepository _tripRepository;

        public DriverService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _microbusRepository = unitOfWork.MicrobusRepository;
            _queueRepository = unitOfWork.QueueRepository;
            _queueItemRepository = unitOfWork.QueueItemRepository;
            _tripRepository = unitOfWork.TripRepository;
            _mapper = mapper;
        }

        public Task<Guid> CheckInAtGateAsync(string qrCode, Guid stationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckOutAtGateAsync(string qrCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EndTripAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Task<DriverDashboardDTO> GetDashboardAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Task<List<QueueItemDTO>> GetDriversBeforeMeAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Task<List<QueueItemDTO>> GetQueueByRouteAsync(Guid stationId, Guid routeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetDailyQueueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Guid> StartTripAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }
    }
}