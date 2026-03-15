using AutoMapper;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Route;

namespace SmartMicrobus.Core.Services.Route
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRouteRepository _routeRepository;
        private readonly IMicrobusRepository _microbusRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;

        public RouteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _routeRepository = _unitOfWork.RouteRepository;
            _microbusRepository = _unitOfWork.MicrobusRepository;
            _stationRepository = _unitOfWork.StationRepository;
            _driverRepository = _unitOfWork.DriverRepository;
        }

        public Task<ApiResponse> GetAllRoutesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetDestinationsByFromAsync(string from)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetMicrobusesAtStationAsync(Guid routeId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetMicrobusesOnTheWayAsync(Guid routeId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetRouteSearchResultAsync(Guid routeId)
        {
            throw new NotImplementedException();
        }
    }
}
