
using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class RouteRepository : GenericRepository<Route>, IRouteRepository
    {
        private readonly ApplicationDbContext _context;
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Route>> GetRoutesByFromAsync(string from)
        {
            return await _context.Routes
                .Where(r => r.FromAr == from || r.FromEn == from)
                .ToListAsync();
        }
    }
}