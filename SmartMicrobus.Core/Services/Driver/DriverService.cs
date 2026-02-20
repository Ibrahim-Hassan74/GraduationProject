using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
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

        public Task<ApiResponse> EndTripAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetDashboardAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetDriversBeforeMeAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetQueueByRouteAsync(Guid stationId, Guid routeId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> ResetDailyQueueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> StartTripAsync(Guid driverId)
        {
            throw new NotImplementedException();
        }
    }
}