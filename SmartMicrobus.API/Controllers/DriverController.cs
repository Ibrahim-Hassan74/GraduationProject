using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.API.Filters;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Driver;
using SmartMicrobus.Core.ServiceContracts.Drivers;
using SmartMicrobus.Core.ServiceContracts.Notification;
using System.Security.Claims;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Driver))]
    public class DriverController(IDriverService driverService, ITripService tripService, ILocationTrackingService locationTrackingService ) : CustomControllerBase
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
        public async Task<IActionResult> DriverQueue()
        {
            var driverId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (driverId is null)
                return Unauthorized(ApiResponseFactory.Unauthorized());

            var response = await driverService.GetMyQueueAsync(Guid.Parse(driverId));

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response.Data);
        }

        [HttpPost("start-trip")]
        public async Task<IActionResult> StartTrip(Guid driverId)
        {
            var response = await tripService.StartTripAsync(driverId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("end-trip")]
        public async Task<IActionResult> EndTrip(Guid driverId)
        {
            var response = await tripService.EndTripAsync(driverId);
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

            var response = await tripService.GetDriverHistoryAsync(Guid.Parse(driverId!), request);

            var result = response as ApiResponseWithData<DriverHistoryResponse>;
            if (!response.Success || result.Data is null)
            {
                return NotFound(ApiResponseFactory.NotFound(response.Message!));
            }

            var pagination = new Pagination<DriverHistoryResponse>(request.PageNumber, request.PageSize, result.Data.TotalCount, result.Data);

            return Ok(pagination);
        }

        [HttpGet("get-by-plate-number")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDriverByPlateNumber([FromQuery] string plateNumber)
        {
            var response = await driverService.GetDriverByPlateNumber(plateNumber);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<DriverResponse>;
            return Ok(result?.Data);
        }

        
        [HttpPost("location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateDriverLocationRequest request)
        {

            var driverId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var updateResponse = await locationTrackingService.UpdateDriverLocationAsync(
                driverId,
                request.Latitude,
                request.Longitude
            );

            if (!updateResponse.Success)
            {
                return BadRequest(updateResponse);
            }

            return Ok(updateResponse);
        }

        [HttpGet("location/{driverId}")]
        [AllowAnonymous]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> GetDriverLocation(Guid driverId)
        {
            var response = await locationTrackingService.GetDriverLocationAsync(driverId);

            if (!response.Success)
            {
                return NotFound(response);
            }
            var result = response as ApiResponseWithData<RouteResultDTO>;
            return Ok(result?.Data);
        }

    }
}
