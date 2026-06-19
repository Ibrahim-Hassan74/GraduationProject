using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;
using System.Linq.Expressions;
using SmartMicrobus.Core.DTO.Microbus;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class MicrobusRepository : GenericRepository<Microbus>, IMicrobusRepository
    {
        private readonly ApplicationDbContext _context;
        public MicrobusRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Microbus>> GetActiveMicrobusesByStationAsync(Guid stationId)
        {
            return _context.Microbuses
                .Include(x => x.Route)
                .Include(x => x.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(m => m.Route.FromStationId == stationId && m.IsActive)
                .ToListAsync();
        }

        public async Task<List<Microbus>> GetAllStationMicrobusesAsync(Guid stationId)
        {
            return await _context.Microbuses
                .Include(x => x.Route)
                .Include(x => x.Driver)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(m => m.Route.FromStationId == stationId)
                .ToListAsync();
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

        public async Task<(List<Microbus> Microbuses, int TotalCount)> GetPaginatedByStationAsync(Guid stationId, MicrobusQuery queryObj)
        {
            var query = _context.Microbuses.Where(m => m.Route.FromStationId == stationId);

            if (!string.IsNullOrWhiteSpace(queryObj.Search))
            {
                var search = queryObj.Search.Trim().ToLower();
                query = query.Where(m =>
                    (m.PlateNumber != null && m.PlateNumber.ToLower().Contains(search)) ||
                    (m.Model != null && m.Model.ToLower().Contains(search)) ||
                    (m.Color != null && m.Color.ToLower().Contains(search)));
            }

            if (queryObj.MinPassengerCount.HasValue)
                query = query.Where(m => m.PassengerCount >= queryObj.MinPassengerCount.Value);

            if (queryObj.MaxPassengerCount.HasValue)
                query = query.Where(m => m.PassengerCount <= queryObj.MaxPassengerCount.Value);

            var isDescending = queryObj.SortOrder == SmartMicrobus.Core.Enums.SortOrderOptions.DESC;

            query = queryObj.SortBy switch
            {
                SmartMicrobus.Core.Enums.MicrobusSortBy.PlateNumber => isDescending
                    ? query.OrderByDescending(m => m.PlateNumber)
                    : query.OrderBy(m => m.PlateNumber),

                SmartMicrobus.Core.Enums.MicrobusSortBy.PassengerCount => isDescending
                    ? query.OrderByDescending(m => m.PassengerCount)
                    : query.OrderBy(m => m.PassengerCount),

                SmartMicrobus.Core.Enums.MicrobusSortBy.Model => isDescending
                    ? query.OrderByDescending(m => m.Model)
                    : query.OrderBy(m => m.Model),

                SmartMicrobus.Core.Enums.MicrobusSortBy.Color => isDescending
                    ? query.OrderByDescending(m => m.Color)
                    : query.OrderBy(m => m.Color),

                _ => query.OrderBy(m => m.PlateNumber)
            };

            var totalCount = await query.CountAsync();

            var microbuses = await query
                .Skip((queryObj.PageNumber - 1) * queryObj.PageSize)
                .Take(queryObj.PageSize)
                .ToListAsync();

            return (microbuses, totalCount);
        }
    }
}