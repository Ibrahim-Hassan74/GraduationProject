using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IFavoriteRouteRepository : IGenericRepository<FavoriteRoute>
    {
        Task<List<FavoriteRoute>> GetByPassengerAsync(Guid passengerId);
        Task<FavoriteRoute?> GetByPassengerAndRouteAsync(Guid passengerId, Guid routeId);
    }
}
