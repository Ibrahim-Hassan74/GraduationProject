
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
        public async Task<List<string>> GetDistinctFromCitiesAsync(bool isArabic)
        {
            return await _context.Routes
                .Select(r => isArabic ? r.FromAr : r.FromEn)
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => c.Trim())
                .GroupBy(c => c.ToLower())
                .Select(g => g.First())
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}