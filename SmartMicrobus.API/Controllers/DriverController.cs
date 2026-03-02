using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts;
using SmartMicrobus.Core.ServiceContracts.Drivers;
using System.Security.Claims;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Driver))]
    public class DriverController(IDriverService driverService, ILogger<DriverController> logger) : CustomControllerBase
    {
        [HttpGet("get-current-postion")]
        public async Task<IActionResult> CurrentPosition()
        {
            var driverId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await driverService.GetDashboardAsync(Guid.Parse(driverId!));
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpGet("get-driver-queue")]
        public async Task<IActionResult> DriverQueue(Guid driverId)
        {
            if (driverId == Guid.Empty)
            {
                return BadRequest("Driver ID is required");
            }

            var response = await driverService.GetMyQueueAsync(driverId);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }

        [HttpPost("start-trip")]
        public async Task<IActionResult> StartTrip(Guid driverId)
        {
            if (driverId == Guid.Empty)
            {
                return BadRequest("Driver ID is required");
            }
            var response = await driverService.StartTripAsync(driverId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }

        [HttpPost("end-trip")]
        public async Task<IActionResult> EndTrip(Guid driverId)
        {
            if (driverId == Guid.Empty)
            {
                return BadRequest("Driver ID is required");
            }
            var response = await driverService.EndTripAsync(driverId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }
    }
}
