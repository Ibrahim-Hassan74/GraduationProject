using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class QueueItemRepository : GenericRepository<QueueItem>, IQueueItemRepository
    {
        private readonly ApplicationDbContext _context;
        public QueueItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<QueueItem?> GetActiveByDriverIdAsync(Guid driverId)
        {
            return await _context.QueueItems
                .Include(x => x.Queue)
                    .ThenInclude(q => q.Route)
                    .Include(x => x.Driver)
                    .ThenInclude(u => u.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.DriverId == driverId &&
                    (x.Status == QueueStatus.Waiting ||
                     x.Status == QueueStatus.YourTurn));
        }

        public async Task<bool> ExistsActiveByDriverIdAsync(Guid driverId)
        {
            return await _context.QueueItems
                .AnyAsync(x =>
                    x.DriverId == driverId &&
                    (x.Status == QueueStatus.Waiting ||
                     x.Status == QueueStatus.YourTurn));
        }

        public async Task<int> GetLastPositionAsync(Guid queueId)
        {
            var last = await _context.QueueItems
                .Where(x => x.QueueId == queueId && x.Status == QueueStatus.Waiting)
                .OrderByDescending(x => x.Position)
                .Select(x => x.Position)
                .FirstOrDefaultAsync();

            return last;
        }

        public async Task<int> CountDriversBeforeAsync(Guid queueId, int position)
        {
            return await _context.QueueItems
                .Where(x =>
                    x.QueueId == queueId &&
                    x.Position < position &&
                    x.Status == QueueStatus.Waiting)
                .CountAsync();
        }

        public async Task<int> CountActiveAsync(Guid queueId)
        {
            return await _context.QueueItems
                .Where(x =>
                    x.QueueId == queueId &&
                    x.Status == QueueStatus.Waiting)
                .CountAsync();
        }

        public async Task<QueueItem?> GetFirstInQueueAsync(Guid queueId)
        {
            return await _context.QueueItems
                .Where(x =>
                    x.QueueId == queueId &&
                    x.Status == QueueStatus.Waiting)
                .OrderBy(x => x.Position)
                .FirstOrDefaultAsync();
        }

        public async Task<List<QueueItem>> GetActiveQueueItemsAsync(Guid queueId)
        {
            return await _context.QueueItems
                .Include(x => x.Microbus)
                .Include(x => x.Driver)
                .ThenInclude(y => y.ApplicationUser)
                .Where(x =>
                    x.QueueId == queueId &&
                    x.Status == QueueStatus.Waiting)
                .OrderBy(x => x.Position)
                .ToListAsync();
        }

        public async Task<List<QueueItem>> GetItemsBeforeAsync(Guid queueId, int currentPosition)
        {
            return await _context.QueueItems
                .Include(x => x.Driver)
                .ThenInclude(u => u.ApplicationUser)
                .Include(x => x.Microbus)
                .Where(x => x.QueueId == queueId &&
                            x.Position < currentPosition &&
                            x.Status == QueueStatus.Waiting)
                .OrderBy(x => x.Position)
                .ToListAsync();
        }
        public async Task<List<QueueItem>> GetAllActiveBeforeDateAsync(DateTimeOffset date)
        {
            return await _context.QueueItems
                .Include(x => x.Driver)
                .Include(x => x.Queue)
                .Where(x =>
                    (x.Status == QueueStatus.Waiting ||
                     x.Status == QueueStatus.YourTurn) &&
                    x.JoinedAt < date)
                .ToListAsync();
        }

        public async Task<List<QueueItem>> GetMicrobusesAtStationAsync(Guid routeId)
        {
            return await _context.QueueItems
                .Include(q => q.Driver)
                    .ThenInclude(d => d.Microbus)
                .Include(q => q.Driver)
                .ThenInclude(x=> x.ApplicationUser)
                .Include(q => q.Queue)
                .Where(q => q.Queue.RouteId == routeId && q.Status == QueueStatus.Waiting)
                .OrderBy(q => q.Position)
                .ToListAsync();
        }
        public async Task<int> GetMicrobusesAtStationCountAsync(Guid routeId)
        {
            return await _context.QueueItems
                .CountAsync(q =>
                    q.Queue.RouteId == routeId &&
                    q.Status == QueueStatus.Waiting);
        }
    }
}