using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Station;
using SmartMicrobus.Core.Enums;
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

        public async Task<StationDashboardDTO> GetDashboardStatsAsync(Guid stationId)
        {
            var todayStart = DateTimeOffset.UtcNow.Date;
            var todayEnd = todayStart.AddDays(1);

            var dto = new StationDashboardDTO();

            dto.AvailableMicrobuses = await _context.QueueItems
                .Where(qi => qi.Queue.StationId == stationId && qi.Status == QueueStatus.Waiting)
                .CountAsync();

            dto.IncomingMicrobuses = await _context.Trips
                .Where(t => t.Route.ToStationId == stationId && t.Status == TripStatus.Started)
                .CountAsync();

            var todayTripsQuery = _context.Trips
                .Where(t => t.StationId == stationId && t.StartedAt >= todayStart && t.StartedAt < todayEnd && t.Status != TripStatus.Cancelled);

            dto.CompletedTripsToday = await todayTripsQuery.CountAsync();
            dto.TotalPassengersToday = await todayTripsQuery.SumAsync(t => (int?)t.PassengerCount) ?? 0;

            dto.DemandByRoute = await todayTripsQuery
                .GroupBy(t => t.Route.ToAr)
                .Select(g => new RouteDemandDTO
                {
                    DestinationName = g.Key,
                    PassengerCount = g.Sum(x => x.PassengerCount)
                })
                .ToListAsync();

            var trips = await todayTripsQuery
                .Select(t => t.StartedAt)
                .ToListAsync();

            dto.TripsOverTime = trips
                .GroupBy(t => t.Hour)
                .Select(g => new HourlyTripVolumeDTO
                {
                    Hour = $"{g.Key:D2}:00",
                    TripCount = g.Count()
                })
                .OrderBy(x => x.Hour)
                .ToList();

            dto.LiveRouteQueues = await _context.QueueItems
                .Where(qi => qi.Queue.StationId == stationId && qi.Status == QueueStatus.Waiting)
                .GroupBy(qi => qi.Queue.Route.ToAr)
                .Select(g => new LiveRouteQueueDTO
                {
                    DestinationName = g.Key,
                    MicrobusesReady = g.Count()
                })
                .ToListAsync();

            return dto;
        }
    }
}