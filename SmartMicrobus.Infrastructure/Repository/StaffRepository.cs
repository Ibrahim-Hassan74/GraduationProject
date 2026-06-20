using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Staff;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        private readonly ApplicationDbContext _context;
        public StaffRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Staff?> GetStaffByUserId(Guid userId)
        {
            return await _context.Staff.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<(List<Staff> Staffs, int TotalCount)> GetPaginatedByStationAsync(Guid stationId, StaffQuery queryObj)
        {
            var query = _context.Staff
                .Include(s => s.User)
                .Where(s => s.StationId == stationId && !s.User.IsDeleted);

            if (!string.IsNullOrWhiteSpace(queryObj.Search))
            {
                var search = queryObj.Search.Trim().ToLower();
                query = query.Where(s => 
                    (s.User.DisplayName != null && s.User.DisplayName.ToLower().Contains(search)) ||
                    (s.User.PhoneNumber != null && s.User.PhoneNumber.ToLower().Contains(search)));
            }

            var isDescending = queryObj.SortOrder == SortOrderOptions.DESC;

            // Default sorting by Name
            query = isDescending 
                ? query.OrderByDescending(s => s.User.DisplayName) 
                : query.OrderBy(s => s.User.DisplayName);

            var totalCount = await query.CountAsync();

            var staffs = await query
                .Skip((queryObj.PageNumber - 1) * queryObj.PageSize)
                .Take(queryObj.PageSize)
                .ToListAsync();

            return (staffs, totalCount);
        }
    }
}
