using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Admin;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IUserRepository
    {
        Task<(List<UserListItem> Items, int TotalCount)> GetPagedUsersWithRolesAsync(GetUsersQuery query);

        Task<UserListItem?> GetUserByIdAsync(Guid id);

        Task<Manager?> GetByIdWithUserAsync(Guid managerId);
    }
}
