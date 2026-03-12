
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
        public async Task<Queue> GetOrCreateQueueAsync(Guid routeId, Guid stationId)
        {
            var queue = await GetByStationAndRouteAsync(stationId, routeId);
            if (queue != null) return queue;

            var newQueue = new Queue
            {
                RouteId = routeId,
                StationId = stationId
            };

            newQueue = await AddAsync(newQueue);

            await _context.SaveChangesAsync();

            return newQueue;
        }
    }
}