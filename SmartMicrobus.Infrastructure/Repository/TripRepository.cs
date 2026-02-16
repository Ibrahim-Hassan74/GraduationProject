using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class TripRepository : GenericRepository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}