using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Trip;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class TripRepository : GenericRepository<Trip>, ITripRepository
    {
        private readonly ApplicationDbContext _context;
        public TripRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Trip?> GetActiveTripAsync(Guid driverId)
        {
            return await _context.Trips
                .Include(x => x.Route)
                .FirstOrDefaultAsync(x =>
                    x.DriverId == driverId &&
                    x.Status == TripStatus.Started);
        }
        public async Task<Trip?> GetTripByDriverIdAsync(Guid driverId)
        {
            return await _context.Trips
                .Include(x => x.Route)
                    .ThenInclude(r => r.FromStation)
                .FirstOrDefaultAsync(x =>
                    x.DriverId == driverId &&
                    x.Status == TripStatus.Started);
        }
        public async Task<TripHistoryResponse> GetDriverTripsAsync(Guid driverId, DriverHistoryRequest request)
        {
            var trips = _context.Trips
                .Include(x => x.Route)
                .Include(x => x.Microbus)
                .Where(x => x.DriverId == driverId &&
                            x.StartedAt >= request.FromDate &&
                            x.StartedAt < request.ToDate &&
                            x.Status == TripStatus.Completed);
            var totalAmount = await trips.SumAsync(x => x.TotalAmount);
            var totalCount = await trips.CountAsync();
            trips = ApplyPagination(trips, request);
            trips = trips.OrderByDescending(x => x.StartedAt);

            var result = await trips.ToListAsync();

            return new TripHistoryResponse(totalAmount, result, totalCount);
        }

        private IQueryable<Trip> ApplyPagination(IQueryable<Trip> trips, DriverHistoryRequest query)
        {
            int pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            int pageSize = query.PageSize < 1 ? 10 : query.PageSize;

            int skipAmount = (pageNumber - 1) * pageSize;

            return trips.Skip(skipAmount).Take(pageSize);
        }

        //public async Task<List<Trip>> GetMicrobusesOnTheWayAsync(Guid routeId)
        //{
        //    return await _context.Trips
        //        .Include(t => t.Microbus)
        //        .Include(t => t.Driver)
        //            .ThenInclude(x => x.ApplicationUser)
        //        .Where(t => t.RouteId == routeId && t.Status == TripStatus.Started)
        //        .ToListAsync();
        //}
        public async Task<List<Trip>> GetMicrobusesOnTheWayAsync(Guid routeId)
        {
            var route = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
                return new List<Trip>();

            var reverseRoute = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.FromStationId == route.ToStationId &&
                    r.ToStationId == route.FromStationId);

            if (reverseRoute == null)
                return new List<Trip>();

            return await _context.Trips
                .Include(t => t.Microbus)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(t =>
                    t.RouteId == reverseRoute.Id &&
                    t.Status == TripStatus.Started)
                .ToListAsync();
        }
        //public async Task<int> GetMicrobusesOnTheWayCountAsync(Guid routeId)
        //{
        //    return await _context.Trips
        //        .CountAsync(t =>
        //            t.RouteId == routeId &&
        //            t.Status == TripStatus.Started);
        //}

        public async Task<int> GetMicrobusesOnTheWayCountAsync(Guid routeId)
        {
            var route = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
                return 0;

            var reverseRoute = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.FromStationId == route.ToStationId &&
                    r.ToStationId == route.FromStationId);

            if (reverseRoute == null)
                return 0;

            return await _context.Trips
                .CountAsync(t =>
                    t.RouteId == reverseRoute.Id &&
                    t.Status == TripStatus.Started);
        }

        public async Task<List<Trip>> GetStationTripsAsync(Guid routeId, Guid stationId)
        {
            return await _context.Trips
                .Include(x => x.Route)
                .Include(x => x.Microbus)
                    .ThenInclude(x => x.Driver)
                .Where(x => x.RouteId == routeId
                         && x.StationId == stationId
                         && x.StartedAt.Date < DateTime.Today
                         && x.Status == TripStatus.Completed)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetTripsByStationAndDateAsync(Guid stationId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _context.Trips
                .Include(t => t.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .Include(t => t.Microbus)
                .Include(t => t.Route)
                .Where(t => t.StationId == stationId && t.StartedAt >= startDate && t.StartedAt <= endDate)
                .ToListAsync();
        }
    }
}