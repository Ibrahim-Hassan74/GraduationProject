using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Microbus;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;
using System.Linq.Expressions;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class MicrobusRepository : GenericRepository<Microbus>, IMicrobusRepository
    {
        private readonly ApplicationDbContext _context;
        public MicrobusRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Microbus?> GetByQrCodeAsync(string qrCode)
        {
            return await _context.Microbuses
                .Include(x => x.Route)
                .Include(x => x.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(x => x.QrCode == qrCode);
        }
        public async Task<Driver?> GetDriverAsync(string plateNumber)
        {
            plateNumber = plateNumber.Trim().ToLower();

            return await _context.Microbuses
                .Where(m => m.PlateNumber != null && m.PlateNumber.ToLower() == plateNumber)
                .Select(m => m.Driver)
                .FirstOrDefaultAsync();
        }
        public async Task<(IEnumerable<Microbus> Microbuses, int TotalCount)> GetFilteredMicrobusesAsync(Guid stationId, MicrobusQuery filter)
        {
            IQueryable<Microbus> query = _context.Microbuses
                .Include(m => m.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .Include(m => m.Route)
                .AsQueryable();

            // Manager Station Filter
            query = query.Where(m =>
                m.Route.FromStationId == stationId ||
                m.Route.ToStationId == stationId);

            // Search
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
            {
                string search = filter.SearchString.Trim().ToLower();

                query = filter.SearchBy switch
                {
                    MicrobusSearchBy.PlateNumber =>
                        query.Where(m =>
                            m.PlateNumber.ToLower().Contains(search)),

                    MicrobusSearchBy.DriverName =>
                        query.Where(m =>
                            m.Driver != null &&
                            m.Driver.ApplicationUser.DisplayName
                                .ToLower()
                                .Contains(search)),

                    MicrobusSearchBy.Model =>
                        query.Where(m =>
                            m.Model.ToLower().Contains(search)),

                    MicrobusSearchBy.Color =>
                        query.Where(m =>
                            m.Color.ToLower().Contains(search)),

                    MicrobusSearchBy.Route =>
                        query.Where(m =>
                            m.Route.FromAr.ToLower().Contains(search) ||
                            m.Route.ToAr.ToLower().Contains(search)),

                    _ => query
                };
            }

            // Filters
            if (filter.IsActive.HasValue)
                query = query.Where(x => x.IsActive == filter.IsActive);

            if (filter.RouteId.HasValue)
                query = query.Where(x => x.RouteId == filter.RouteId);

            if (filter.DriverId.HasValue)
                query = query.Where(x => x.DriverId == filter.DriverId);

            // Sorting
            query = filter.SortBy switch
            {
                MicrobusSortBy.DriverName =>
                    filter.OrderOptions == SortOrderOptions.ASC
                        ? query.OrderBy(x => x.Driver!.ApplicationUser.DisplayName)
                        : query.OrderByDescending(x => x.Driver!.ApplicationUser.DisplayName),

                MicrobusSortBy.PassengerCount =>
                    filter.OrderOptions == SortOrderOptions.ASC
                        ? query.OrderBy(x => x.PassengerCount)
                        : query.OrderByDescending(x => x.PassengerCount),

                MicrobusSortBy.RouteName =>
                    filter.OrderOptions == SortOrderOptions.ASC
                        ? query.OrderBy(x => x.Route.FromAr)
                        : query.OrderByDescending(x => x.Route.FromAr),

                MicrobusSortBy.Model =>
                    filter.OrderOptions == SortOrderOptions.ASC
                        ? query.OrderBy(x => x.Model)
                        : query.OrderByDescending(x => x.Model),

                MicrobusSortBy.Color =>
                    filter.OrderOptions == SortOrderOptions.ASC
                        ? query.OrderBy(x => x.Color)
                        : query.OrderByDescending(x => x.Color),

                _ =>
                    filter.OrderOptions == SortOrderOptions.ASC
                        ? query.OrderBy(x => x.PlateNumber)
                        : query.OrderByDescending(x => x.PlateNumber)
            };

            var totalCount = await query.CountAsync();

            var microbuses = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (microbuses, totalCount);
        }

        public async Task<Microbus?> GetByIdWithDetailsAsync(Guid microbusId, Guid stationId)
        {
            return await _context.Microbuses
                .Include(m => m.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .Include(m => m.Route)
                .FirstOrDefaultAsync(m =>
                    m.Id == microbusId &&
                    (m.Route.FromStationId == stationId ||
                     m.Route.ToStationId == stationId));
        }
    }
}