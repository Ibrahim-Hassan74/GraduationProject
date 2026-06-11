using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Manager;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts.Admin;

namespace SmartMicrobus.API.Controllers.Admin
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    [Route("api/admin")]
    public class AdminController(IAdminService adminService) : CustomControllerBase
    {
        [HttpPost("add-manager")]
        public async Task<IActionResult> AddManager([FromBody] RegisterManagerDTO registerManagerDTO)
        {
            var result = await adminService.AddManagerAsync(registerManagerDTO);
            return ToActionResult(result);
        }
    }
}
