using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class ReportReasonRepository : GenericRepository<ReportReason>, IReportReasonRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportReasonRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }

       
        public async Task<List<ReportReason>> GetByIdsAsync(List<int> ids)
        {
            return await _context.ReportReasons.Where(r => ids.Contains(r.Id)).ToListAsync();
        }
    }
}
