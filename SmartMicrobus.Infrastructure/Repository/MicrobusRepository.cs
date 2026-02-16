
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class MicrobusRepository : GenericRepository<Microbus>, IMicrobusRepository
    {
        public MicrobusRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}