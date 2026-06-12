using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Manager;

namespace SmartMicrobus.Core.ServiceContracts.Admin
{
    public interface IAdminService
    {
        Task<ApiResponse> AddManagerAsync(RegisterManagerDTO registerManagerDTO);
    }
}
