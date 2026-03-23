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
        public async Task<Microbus?> GetByQrCodeAsync(string qrCode)
        {
            return await _context.Microbuses
                .Include(x => x.Route)
                .Include(x => x.Driver)
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