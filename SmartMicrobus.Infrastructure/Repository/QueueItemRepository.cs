
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class QueueItemRepository : GenericRepository<QueueItem>, IQueueItemRepository
    {
        public QueueItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}