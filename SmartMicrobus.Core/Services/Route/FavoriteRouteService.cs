using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.ServiceContracts.Route;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Helper;
using AutoMapper;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Resources.Services.Route;

namespace SmartMicrobus.Core.Services.Route
{
    public class FavoriteRouteService : IFavoriteRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFavoriteRouteRepository _favoriteRouteRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<FavoriteRouteService> _localizer;

        public FavoriteRouteService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IStringLocalizer<FavoriteRouteService> localizer)
        {
            _unitOfWork = unitOfWork;
            _favoriteRouteRepository = _unitOfWork.FavoriteRouteRepository;
            _routeRepository = _unitOfWork.RouteRepository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ApiResponse> AddToFavoritesAsync(Guid passengerId, Guid routeId)
        {
            if (passengerId == Guid.Empty || routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest(_localizer["PassengerIdAndRouteIdRequired"]);

            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route == null)
                return ApiResponseFactory.NotFound(_localizer["RouteNotFound"]);

            var existing = await _favoriteRouteRepository.GetByPassengerAndRouteAsync(passengerId, routeId);
            if (existing != null)
                return ApiResponseFactory.Conflict(_localizer["RouteAlreadyInFavorites"]);

            var fav = new FavoriteRoute
            {
                PassengerId = passengerId,
                RouteId = routeId
            };

            await _favoriteRouteRepository.AddAsync(fav);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(_localizer["RouteAddedToFavorites"]);
        }

        public async Task<ApiResponse> GetFavoriteRoutesAsync(Guid passengerId)
        {
            if (passengerId == Guid.Empty)
                return ApiResponseFactory.BadRequest(_localizer["PassengerIdRequired"]);

            var favs = await _favoriteRouteRepository.GetByPassengerAsync(passengerId);

            if (favs == null || !favs.Any())
                return ApiResponseFactory.Success(_localizer["NoFavoriteRoutesFound"], new List<FavoriteRouteResponse>());

            var resp = _mapper.Map<List<FavoriteRouteResponse>>(favs);

            return ApiResponseFactory.Success(_localizer["FavoriteRoutesRetrieved"], resp);
        }

        public async Task<ApiResponse> IsFavoriteAsync(Guid passengerId, Guid routeId)
        {
            if (passengerId == Guid.Empty || routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest(_localizer["PassengerIdAndRouteIdRequired"]);

            var existing = await _favoriteRouteRepository.GetByPassengerAndRouteAsync(passengerId, routeId);
            var isFav = existing != null;

            return ApiResponseFactory.Success(_localizer["IsFavoriteRetrieved"], isFav);
        }

        public async Task<ApiResponse> RemoveFromFavoritesAsync(Guid passengerId, Guid routeId)
        {
            if (passengerId == Guid.Empty || routeId == Guid.Empty)
                return ApiResponseFactory.BadRequest(_localizer["PassengerIdAndRouteIdRequired"]);

            var existing = await _favoriteRouteRepository.GetByPassengerAndRouteAsync(passengerId, routeId);
            if (existing == null)
                return ApiResponseFactory.NotFound(_localizer["FavoriteRouteNotFound"]);

            await _favoriteRouteRepository.DeleteAsync(existing.Id);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(_localizer["RouteRemovedFromFavorites"]);
        }
    }
}