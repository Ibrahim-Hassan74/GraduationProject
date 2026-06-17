using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Station;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IStationRepository : IGenericRepository<Station>
    {
        Task<Station?> GetNearestStationAsync(double lat, double lng);
        Task AddStationAsync(Station station);
        Task<StationDashboardDTO> GetDashboardStatsAsync(Guid stationId);
    }
}