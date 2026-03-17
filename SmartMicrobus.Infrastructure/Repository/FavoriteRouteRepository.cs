using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class FavoriteRouteRepository : GenericRepository<FavoriteRoute>, IFavoriteRouteRepository
    {
        private readonly ApplicationDbContext _context;
        public FavoriteRouteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<FavoriteRoute>> GetByPassengerAsync(Guid passengerId)
        {
            return await _context.FavoriteRoutes
                .Include(x => x.Route)
                .Where(x => x.PassengerId == passengerId)
                .ToListAsync();
        }

        public async Task<FavoriteRoute?> GetByPassengerAndRouteAsync(Guid passengerId, Guid routeId)
        {
            return await _context.FavoriteRoutes
                .FirstOrDefaultAsync(x => x.PassengerId == passengerId && x.RouteId == routeId);
        }
    }
}
