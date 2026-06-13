using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class ReportRepository : GenericRepository<DriverReport>, IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<DriverReport> Items, int TotalCount)> GetPagedReportsForAdminAsync(GetReportsQuery query, Guid stationId)
        {
            var queryable = _context.DriverReports
                .Include(x => x.Driver)
                    .ThenInclude(d => d.Microbus)
                    .ThenInclude(m => m.Route)
                    .Where(x => x.Driver != null && x.Driver.Microbus != null && x.Driver.Microbus.Route != null && (x.Driver.Microbus.Route.FromStationId == stationId || x.Driver.Microbus.Route.ToStationId == stationId))
                .AsQueryable();



            // Apply PlateNumber filter
            if (!string.IsNullOrWhiteSpace(query.PlateNumber))
            {
                var plateLower = query.PlateNumber.Trim().ToLower();
                queryable = queryable.Where(r => r.PlateNumber.ToLower().Contains(plateLower));
            }

            // Apply date range filters
            if (query.FromDate.HasValue)
            {
                var fromDateOffset = new DateTimeOffset(query.FromDate.Value.Date, TimeSpan.Zero);
                queryable = queryable.Where(r => r.CreatedAt >= fromDateOffset);
            }

            if (query.ToDate.HasValue)
            {
                var toDateOffset = new DateTimeOffset(query.ToDate.Value.Date.AddDays(1), TimeSpan.Zero);
                queryable = queryable.Where(r => r.CreatedAt < toDateOffset);
            }

            if (query.Status.HasValue)
            {
                queryable = queryable.Where(r => r.Status == query.Status.Value);
            }

            // Get total count before pagination
            var totalCount = await queryable.CountAsync();
            if (totalCount == 0)
            {
                return (new List<DriverReport>(), 0);
            }


            switch (query.OrderByOptions)
            {
                case OrderByOptions.CreatedAt:
                    queryable = query.OrderOptions == SortOrderOptions.DESC
                        ? queryable.OrderByDescending(r => r.CreatedAt)
                        : queryable.OrderBy(r => r.CreatedAt);
                    break;

                default:
                    queryable = query.OrderOptions == SortOrderOptions.DESC
                        ? queryable.OrderByDescending(r => r.ResolvedAt)
                        : queryable.OrderBy(r => r.ResolvedAt);
                    break;
            }


            // Apply pagination
            var skip = (query.PageNumber - 1) * query.PageSize;
            var items = await queryable
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<DriverReport?> GetByIdWithReasonsAsync(Guid id, Guid stationId)
        {
            return await _context.DriverReports
                .Include(x => x.Driver)
                    .ThenInclude(d => d.Microbus)
                    .ThenInclude(m => m.Route)
                .Include(x => x.Passenger)
                    .ThenInclude(p => p.ApplicationUser)
                .Include(x => x.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .Include(r => r.Reasons)
                    .ThenInclude(rr => rr.ReportReason)
                .Where(x => x.Driver != null && x.Driver.Microbus != null && x.Driver.Microbus.Route != null && (x.Driver.Microbus.Route.FromStationId == stationId || x.Driver.Microbus.Route.ToStationId == stationId))
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<DriverReport?> GetByIdWithReasonsAsync(Guid id)
        {
            return await _context.DriverReports
                .Include(r => r.Reasons)
                    .ThenInclude(rr => rr.ReportReason)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<bool> HasRecentReportAsync(Guid passengerId, string plate)
        {
            plate = plate.Trim().ToLower();
            var threshold = DateTimeOffset.UtcNow.AddDays(-7);

            return await _context.DriverReports.AnyAsync(r =>
                r.PassengerId == passengerId &&
                r.PlateNumber.ToLower() == plate &&
                r.CreatedAt >= threshold
            );
        }

        public async Task<(List<DriverReport> Items, int TotalCount)> GetPagedReportsAsync(Guid passengerId, GetReportsQuery query)
        {
            var queryable = _context.DriverReports
                .Where(r => r.PassengerId == passengerId)
                .Include(r => r.Reasons)
                    .ThenInclude(rr => rr.ReportReason)
                .AsQueryable();

            // Apply PlateNumber filter
            if (!string.IsNullOrWhiteSpace(query.PlateNumber))
            {
                var plateLower = query.PlateNumber.Trim().ToLower();
                queryable = queryable.Where(r => r.PlateNumber.ToLower().Contains(plateLower));
            }

            // Apply date range filters
            if (query.FromDate.HasValue)
            {
                var fromDateOffset = new DateTimeOffset(query.FromDate.Value.Date, TimeSpan.Zero);
                queryable = queryable.Where(r => r.CreatedAt >= fromDateOffset);
            }

            if (query.ToDate.HasValue)
            {
                var toDateOffset = new DateTimeOffset(query.ToDate.Value.Date.AddDays(1), TimeSpan.Zero);
                queryable = queryable.Where(r => r.CreatedAt < toDateOffset);
            }

            // Get total count before pagination
            var totalCount = await queryable.CountAsync();

            // Apply pagination
            var skip = (query.PageNumber - 1) * query.PageSize;
            var items = await queryable
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
