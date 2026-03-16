using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Route
{
    public interface IFavoriteRouteService
    {
        Task<ApiResponse> GetFavoriteRoutesAsync(Guid passengerId); // return List of FavoriteRouteResponse

        Task<ApiResponse> AddToFavoritesAsync(Guid passengerId, Guid routeId); // return ApiResponse indicating success or failure of the operation

        Task<ApiResponse> RemoveFromFavoritesAsync(Guid passengerId, Guid routeId); // return ApiResponse indicating success or failure of the operation

        Task<ApiResponse> IsFavoriteAsync(Guid passengerId, Guid routeId); // return ApiResponse indicating whether the route is a favorite for the passenger
    }
}
