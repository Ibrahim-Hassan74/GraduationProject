
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class StationRepository : GenericRepository<Station>, IStationRepository
    {
        public StationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}