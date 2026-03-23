using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
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
    }
}
