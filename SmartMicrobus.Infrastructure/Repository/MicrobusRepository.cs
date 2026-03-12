using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

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
    }
}