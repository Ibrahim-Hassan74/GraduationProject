using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts.Manager;
using SmartMicrobus.Core.DTO.Microbus;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Manager))]
    public class ManagerController(IManagerService managerService) : CustomControllerBase
    {
        [HttpPost]
        [Route("add-driver")]
        public async Task<IActionResult> AddDriver([FromBody] DriverAddRequest driverAddRequest)
        {
            if (driverAddRequest == null)
                return BadRequest("driver data cannot be empty");

            var result = await managerService.AddDriverAsync(driverAddRequest);

            if (result is null)
                return BadRequest("failed to add driver");

            return Ok(result);
        }

        [HttpPost]
        [Route("add-microbus")]
        public async Task<IActionResult> AddMicrobus([FromBody] MicrobusAddRequest microbusAddRequest)
        {
            if (microbusAddRequest == null)
                return BadRequest("microbus data cannot be empty");

            var result = await managerService.AddMicrobusAsync(microbusAddRequest);

            if (result is null)
                return BadRequest("failed to add microbus");

            return Ok(result);
        }

        [HttpPost]
        [Route("assign-driver-microbus")]
        public async Task<IActionResult> AssignDriverToMicrobus([FromBody] DriverAssignRequest driverAssignRequest)
        {
            if (driverAssignRequest == null)
                return BadRequest("assignment data cannot be empty");

            var result = await managerService.AssignDriverToMicrobusAsync(driverAssignRequest);

            if (result is null)
                return BadRequest("failed to assign driver");

            return Ok(result);
        }
    }
}
