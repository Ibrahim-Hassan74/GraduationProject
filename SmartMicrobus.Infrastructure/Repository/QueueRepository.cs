
using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class QueueRepository : GenericRepository<Queue>, IQueueRepository
    {
        private readonly ApplicationDbContext _context;
        public QueueRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Queue?> GetByStationAndRouteAsync(Guid stationId, Guid routeId)
        {
            return await _context.Queues
                .Include(q => q.Route)
                .FirstOrDefaultAsync(q =>
                    q.StationId == stationId &&
                    q.RouteId == routeId);
        }
        public async Task<Queue?> GetByIdWithItemsAsync(Guid queueId)
        {
            return await _context.Queues
                .Include(q => q.Route)
                .Include(q => q.Items)
                    .ThenInclude(i => i.Driver)
                .FirstOrDefaultAsync(q => q.Id == queueId);
        }
    }
}