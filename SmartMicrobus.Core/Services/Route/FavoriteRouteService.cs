using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.ServiceContracts.Route;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Helper;
using AutoMapper;

namespace SmartMicrobus.Core.Services.Route
{
    public class FavoriteRouteService : IFavoriteRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFavoriteRouteRepository _favoriteRouteRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IMapper _mapper;

        public FavoriteRouteService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _favoriteRouteRepository = _unitOfWork.FavoriteRouteRepository;
            _routeRepository = _unitOfWork.RouteRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> AddToFavoritesAsync(Guid passengerId, Guid routeId)
        {
            if (passengerId == Guid.Empty || routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("PassengerId and RouteId are required.");

            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null)
                return ApiResponseFactory.NotFound("Route not found.");

            var existing = await _favoriteRouteRepository.GetByPassengerAndRouteAsync(passengerId, routeId);
            if (existing != null)
                return ApiResponseFactory.Conflict("Route is already in favorites.");

            var fav = new FavoriteRoute
            {
                PassengerId = passengerId,
                RouteId = routeId
            };

            await _favoriteRouteRepository.AddAsync(fav);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Route added to favorites.");
        }

        public async Task<ApiResponse> GetFavoriteRoutesAsync(Guid passengerId)
        {
            if (passengerId == Guid.Empty)
                return ApiResponseFactory.BadRequest("PassengerId is required.");

            var favs = await _favoriteRouteRepository.GetByPassengerAsync(passengerId);
            if (favs == null || !favs.Any())
                return ApiResponseFactory.Success("No favorite routes found.",favs);

            var resp = _mapper.Map<List<FavoriteRouteResponse>>(favs);

            return ApiResponseFactory.Success("Favorite routes retrieved.", resp);
        }

        public async Task<ApiResponse> IsFavoriteAsync(Guid passengerId, Guid routeId)
        {
            if (passengerId == Guid.Empty || routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("PassengerId and RouteId are required.");

            var existing = await _favoriteRouteRepository.GetByPassengerAndRouteAsync(passengerId, routeId);
            var isFav = existing != null;

            return ApiResponseFactory.Success("Is favorite retrieved.", isFav);
        }

        public async Task<ApiResponse> RemoveFromFavoritesAsync(Guid passengerId, Guid routeId)
        {
            if (passengerId == Guid.Empty || routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest("PassengerId and RouteId are required.");

            var existing = await _favoriteRouteRepository.GetByPassengerAndRouteAsync(passengerId, routeId);
            if (existing == null)
                return ApiResponseFactory.NotFound("Favorite route not found.");

            await _favoriteRouteRepository.DeleteAsync(existing.Id);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Route removed from favorites.");
        }
    }
}
