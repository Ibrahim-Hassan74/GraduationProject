using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class DriverRepository(ApplicationDbContext context) : GenericRepository<Driver>(context) ,IDriverRepository
    {
    }
}
