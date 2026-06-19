using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Admin;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Manager;

namespace SmartMicrobus.Core.ServiceContracts.Admin
{
    public interface IAdminService
    {
        Task<ApiResponse> AddManagerAsync(RegisterManagerDTO registerManagerDTO);
        Task<ApiResponse> GetUsersAsync(GetUsersQuery query);
        Task<ApplicationUserResponse?> GetUserByIdAsync(Guid userId);
        Task<ApiResponse> LockAccountAsync(Guid userId);
        Task<ApiResponse> UnlockAccountAsync(Guid userId);
    }
}
