using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
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
                return BadRequest(response);
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
                return BadRequest(response);
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
                return BadRequest(response);
            }
            return Ok(response);
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
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetDriverHistory([FromQuery] DriverHistoryRequest request)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await driverService.GetDriverHistoryAsync(Guid.Parse(driverId!), request);

            var result = response as ApiResponseWithData<DriverHistoryResponse>;
            if(!response.Success || result.Data is null) 
            {
                return NotFound(ApiResponseFactory.NotFound(result?.Message ?? "No trips found for the selected period"));
            }

            var pagination = new Pagination<DriverHistoryResponse>(request.PageNumber, request.PageSize, result.Data.TotalCount, result.Data);

            return Ok(pagination);
        }
    }
}
