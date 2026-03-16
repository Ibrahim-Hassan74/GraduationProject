using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.ServiceContracts.Route;

namespace SmartMicrobus.Core.Services.Route
{
    public class FavoriteRouteService : IFavoriteRouteService
    {
        public Task<ApiResponse> AddToFavoritesAsync(Guid passengerId, Guid routeId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetFavoriteRoutesAsync(Guid passengerId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> IsFavoriteAsync(Guid passengerId, Guid routeId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> RemoveFromFavoritesAsync(Guid passengerId, Guid routeId)
        {
            throw new NotImplementedException();
        }
    }
}
