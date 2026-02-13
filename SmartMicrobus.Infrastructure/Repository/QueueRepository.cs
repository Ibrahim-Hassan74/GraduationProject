
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class QueueRepository : GenericRepository<Queue>, IQueueRepository
    {
        public QueueRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}