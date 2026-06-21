using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Admin;
using SmartMicrobus.Core.DTO.Manager;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Admin;
using SmartMicrobus.Core.Services.Admin;

namespace SmartMicrobus.API.Controllers.Admin
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class AdminController(IAdminService adminService) : CustomControllerBase
    {
        [HttpPost("add-manager")]
        public async Task<IActionResult> AddManager([FromBody] RegisterManagerDTO registerManagerDTO)
        {
            var result = await adminService.AddManagerAsync(registerManagerDTO);
            return ToActionResult(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var result = await adminService.GetUsersAsync(query);
            return ToActionResult(result);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound(ApiResponseFactory.NotFound("User not found."));
            return Ok(ApiResponseFactory.Success("User retrieved successfully", user));
        }

        [HttpPost("users/{id}/lock")]
        public async Task<IActionResult> LockUser(Guid id)
        {
            var result = await adminService.LockAccountAsync(id);
            return ToActionResult(result);
        }

        [HttpPost("users/{id}/unlock")]
        public async Task<IActionResult> UnlockUser(Guid id)
        {
            var result = await adminService.UnlockAccountAsync(id);
            return ToActionResult(result);
        }

        [HttpDelete("managers/{managerId:guid}")]
        public async Task<IActionResult> DeleteManager(Guid managerId)
        {
            var response = await adminService.DeleteManagerAsync(managerId);

            return ToActionResult(response);
        }

        [HttpPut("managers/{managerId:guid}/station")]
        public async Task<IActionResult> UpdateManagerStation(Guid managerId, UpdateManagerStationDTO dto)
        {
            var response = await adminService
                .UpdateManagerStationAsync(managerId, dto);

            return ToActionResult(response);
        }
    }
}
