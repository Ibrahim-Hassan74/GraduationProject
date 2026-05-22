using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class StationRepository : GenericRepository<Station>, IStationRepository
    {
        private readonly ApplicationDbContext _context;
        public StationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddStationAsync(Station station)
        {
            station.Location = new Point(station.Longitude, station.Latitude)
            {
                SRID = 4326
            };

            await _context.Stations.AddAsync(station);
        }
        public async Task<Station?> GetNearestStationAsync(double lat, double lng)
        {
            var userLocation = new Point(lng, lat) { SRID = 4326 };

            return await _context.Stations
                .Where(s => s.Location != null)
                .OrderBy(s => s.Location.Distance(userLocation))
                .FirstOrDefaultAsync();
        }
    }
}