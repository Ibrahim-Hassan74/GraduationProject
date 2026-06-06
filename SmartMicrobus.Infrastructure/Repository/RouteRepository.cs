
using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;
using System.Globalization;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class RouteRepository : GenericRepository<Route>, IRouteRepository
    {
        private readonly ApplicationDbContext _context;
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<List<Route>> GetRoutesByFromAsync(string from)
        //{
        //    return await _context.Routes
        //        .Where(r => r.FromAr == from || r.FromEn == from)
        //        .ToListAsync();
        //}

        public async Task<List<Route>> GetRoutesByFromAsync(Guid fromStationId)
        {
            return await _context.Routes
                .Where(r => r.FromStationId == fromStationId)
                .OrderBy(r => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? r.ToAr : r.ToEn)
                .ToListAsync();
        }

        //public async Task<List<string>> GetDistinctFromCitiesAsync(bool isArabic)
        //{
        //    return await _context.Routes
        //        .Select(r => isArabic ? r.FromAr : r.FromEn)
        //        .Where(c => !string.IsNullOrEmpty(c))
        //        .Select(c => c.Trim())
        //        .GroupBy(c => c.ToLower())
        //        .Select(g => g.First())
        //        .OrderBy(c => c)
        //        .ToListAsync();
        //}
        public async Task<List<RouteLocationResponse>> GetDistinctFromCitiesAsync(bool isArabic)
        {
            return await _context.Routes
                .Where(r => !string.IsNullOrEmpty(isArabic ? r.FromAr : r.FromEn))
                .GroupBy(r => isArabic ? r.FromAr : r.FromEn)
                .Select(g => new RouteLocationResponse
                {
                    CityName = g.Key,
                    StationId = g.First().FromStationId
                })
                .OrderBy(x => x.CityName)
                .ToListAsync();
        }

        public async Task<List<Route>> GetRoutesByLineAsync(Guid routeId)
        {
            var baseRoute = await _context.Routes
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (baseRoute == null)
                return new List<Route>();

            var fromId = baseRoute.FromStationId;
            var toId = baseRoute.ToStationId;

            return await _context.Routes
                .Where(r =>
                    (r.FromStationId == fromId && r.ToStationId == toId) ||
                    (r.FromStationId == toId && r.ToStationId == fromId)
                )
                .ToListAsync();
        }

        public async Task<Route?> GetReverseRouteAsync(Route baseRoute)
        {
            return await _context.Routes
                .FirstOrDefaultAsync(r =>
                    r.FromStationId == baseRoute.ToStationId &&
                    r.ToStationId == baseRoute.FromStationId);
        }
    }
}