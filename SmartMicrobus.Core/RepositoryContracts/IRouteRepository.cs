
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IRouteRepository : IGenericRepository<Route>
    {
        Task<List<Route>> GetRoutesByFromAsync(string from);
        Task<List<string>> GetDistinctFromCitiesAsync(bool isArabic);
        Task<List<Route>> GetRoutesByLineAsync(Guid routeId);
    }
}