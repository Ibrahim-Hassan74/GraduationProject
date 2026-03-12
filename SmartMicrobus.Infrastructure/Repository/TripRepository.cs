using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
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
        public async Task<List<Trip>?> GetDriverTripsAsync(Guid driverId, DateTime from, DateTime to)
        {
            return await _context.Trips
                .Include(x => x.Route)
                .Include(x => x.Microbus)
                .Where(x => x.Microbus.DriverId == driverId &&
                            x.StartedAt >= from &&
                            x.StartedAt < to &&
                            x.Status == TripStatus.Completed)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }
    }
}