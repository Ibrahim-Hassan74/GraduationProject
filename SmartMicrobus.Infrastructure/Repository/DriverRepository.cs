using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class DriverRepository(ApplicationDbContext context) : GenericRepository<Driver>(context) ,IDriverRepository
    {
        public Task<Driver?> GetDriverByLicense(string licenseNumber)
        {
            var driver = context.Drivers
                .Include(d => d.ApplicationUser)
                .Include(d => d.Microbus)
                    .ThenInclude(r=> r.Route)
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
            return driver;
        }

        public async Task<Driver?>GetDriverByPlateNumber(string plateNumber)
        {
            var driver = await context.Drivers
                .Include(d => d.ApplicationUser)
                .Include(d => d.Microbus).ThenInclude(r=>r.Route)
                .FirstOrDefaultAsync(d => d.Microbus.PlateNumber == plateNumber);
            return driver;
        }
    }
}
