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
        public async Task<TripHistoryResponse> GetDriverTripsAsync(Guid driverId, DriverHistoryRequest request)
        {
            var trips = _context.Trips
                .Include(x => x.Route)
                .Include(x => x.Microbus)
                .Where(x => x.Microbus.DriverId == driverId &&
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
    }
}