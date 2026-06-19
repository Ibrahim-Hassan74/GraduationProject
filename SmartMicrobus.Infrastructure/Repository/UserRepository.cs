using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.DTO.Admin;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<UserListItem> Items, int TotalCount)> GetPagedUsersWithRolesAsync(GetUsersQuery query)
        {
            var search = query.Search?.Trim().ToLower();
            var roleFilter = query.Role?.ToString();


            IQueryable<UserListItem> baseQuery =
                from u in _context.Users.AsNoTracking()

                join ur in _context.UserRoles
                    on u.Id equals ur.UserId into userRoles
                from ur in userRoles.DefaultIfEmpty()

                join r in _context.Roles
                    on ur.RoleId equals r.Id into roles
                from r in roles.DefaultIfEmpty()

                select new UserListItem
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    PhoneNumber = u.PhoneNumber,
                    PhotoName = u.Photo != null ? u.Photo.ImageName : null,
                    IsDeleted = u.IsDeleted,
                    IsConfirmed = u.PhoneNumberConfirmed,
                    LockoutEnd = u.LockoutEnd,
                    Role = r != null ? r.Name : null
                };

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                baseQuery = baseQuery.Where(x =>
                    (x.DisplayName ?? string.Empty).ToLower().Contains(search) ||
                    (x.PhoneNumber ?? string.Empty).ToLower().Contains(search));
            }

            // Filter By Role
            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                baseQuery = baseQuery.Where(x => x.Role == roleFilter);
            }

            // Count
            var totalCount = await baseQuery.CountAsync();

            // Sorting
            var sortDesc = query.SortOrder == SortOrderOptions.DESC;

            baseQuery = query.SortBy switch
            {
                UsersSortBy.Role =>
                    sortDesc
                        ? baseQuery.OrderByDescending(x => x.Role)
                        : baseQuery.OrderBy(x => x.Role),

                UsersSortBy.Name =>
                    sortDesc
                        ? baseQuery.OrderByDescending(x => x.DisplayName)
                        : baseQuery.OrderBy(x => x.DisplayName),

                _ =>
                    sortDesc
                        ? baseQuery.OrderByDescending(x => x.DisplayName)
                        : baseQuery.OrderBy(x => x.DisplayName)
            };

            var page = Math.Max(1, query.PageNumber);
            var size = Math.Max(5, query.PageSize);

            var items = await baseQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (items, totalCount);


        }

        public async Task<UserListItem?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserListItem
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    PhoneNumber = u.PhoneNumber,
                    PhotoName = u.Photo != null ? u.Photo.ImageName : null,
                    IsDeleted = u.IsDeleted,
                    LockoutEnd = u.LockoutEnd,
                    Role = _context.UserRoles
                                .Where(ur => ur.UserId == u.Id)
                                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                                .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return user;
        }
    }
}
