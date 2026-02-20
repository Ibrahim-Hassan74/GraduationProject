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
                .FirstOrDefaultAsync(x =>
                    x.DriverId == driverId &&
                    x.Status == TripStatus.Started);
        }
    }
}