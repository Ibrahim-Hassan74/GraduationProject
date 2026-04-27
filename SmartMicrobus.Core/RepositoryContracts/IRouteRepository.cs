
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Route;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IRouteRepository : IGenericRepository<Route>
    {
        //Task<List<Route>> GetRoutesByFromAsync(string from);
        //Task<List<string>> GetDistinctFromCitiesAsync(bool isArabic);
        Task<List<Route>> GetRoutesByFromAsync(Guid fromStationId);
        Task<List<RouteLocationResponse>> GetDistinctFromCitiesAsync(bool isArabic);
        Task<List<Route>> GetRoutesByLineAsync(Guid routeId);
        Task<Route?> GetReverseRouteAsync(Route baseRoute);
    }
}