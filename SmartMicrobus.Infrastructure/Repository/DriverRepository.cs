using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class DriverRepository(ApplicationDbContext context) : IDriverRepository
    {
        public async Task<Driver> AddDriverAsync(Driver driver)
        {
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();
            return driver;
        }
    }
}
