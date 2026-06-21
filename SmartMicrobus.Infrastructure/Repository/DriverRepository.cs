using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.Enums;
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

        public async Task<List<Driver>> GetDriversByStationAsync(Guid stationId)
        {
            return await context.Drivers
                .Include(d => d.ApplicationUser)
                .Include(d => d.Microbus)
                    .ThenInclude(m => m.Route)
                .Where(d => d.Microbus != null && 
                           (d.Microbus.Route.FromStationId == stationId || d.Microbus.Route.ToStationId == stationId))
                .ToListAsync();
        }

        public async Task<(List<Driver> Drivers, int TotalCount)> GetPaginatedByStationAsync(Guid stationId, DriverQuery queryObj)
        {
            var query = context.Drivers
                .Include(d => d.ApplicationUser)
                .Include(d => d.Microbus)
                    .ThenInclude(m => m.Route)
                .Where(d => d.Microbus != null && 
                           (d.Microbus.Route.FromStationId == stationId));

            if (!string.IsNullOrWhiteSpace(queryObj.Search))
            {
                var search = queryObj.Search.Trim().ToLower();
                query = query.Where(d =>
                    (d.ApplicationUser.DisplayName != null && d.ApplicationUser.DisplayName.ToLower().Contains(search)) ||
                    (d.LicenseNumber != null && d.LicenseNumber.ToLower().Contains(search)) ||
                    (d.Microbus.PlateNumber != null && d.Microbus.PlateNumber.ToLower().Contains(search)) ||
                    (d.Microbus.Route.FromEn != null && d.Microbus.Route.FromEn.ToLower().Contains(search)) ||
                    (d.Microbus.Route.FromAr != null && d.Microbus.Route.FromAr.ToLower().Contains(search)) ||
                    (d.Microbus.Route.ToEn != null && d.Microbus.Route.ToEn.ToLower().Contains(search)) ||
                    (d.Microbus.Route.ToAr != null && d.Microbus.Route.ToAr.ToLower().Contains(search)));
            }

            var isDescending = queryObj.SortOrder == SortOrderOptions.DESC;

            query = queryObj.SortBy switch
            {
                DriverSortBy.DriverName => isDescending
                    ? query.OrderByDescending(d => d.ApplicationUser.DisplayName)
                    : query.OrderBy(d => d.ApplicationUser.DisplayName),

                DriverSortBy.LicenseNumber => isDescending
                    ? query.OrderByDescending(d => d.LicenseNumber)
                    : query.OrderBy(d => d.LicenseNumber),

                DriverSortBy.PlateNumber => isDescending
                    ? query.OrderByDescending(d => d.Microbus.PlateNumber)
                    : query.OrderBy(d => d.Microbus.PlateNumber),

                _ => query.OrderBy(d => d.ApplicationUser.DisplayName)
            };

            var totalCount = await query.CountAsync();

            var drivers = await query
                .Skip((queryObj.PageNumber - 1) * queryObj.PageSize)
                .Take(queryObj.PageSize)
                .ToListAsync();

            return (drivers, totalCount);
        }
    }
}
