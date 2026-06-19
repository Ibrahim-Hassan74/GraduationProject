using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
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
    }
}