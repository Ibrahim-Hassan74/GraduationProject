
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IStationRepository : IGenericRepository<Station>
    {
        Task<Station?> GetNearestStationAsync(double lat, double lng);
        Task AddStationAsync(Station station);
    }
}