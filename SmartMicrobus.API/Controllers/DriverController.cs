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
        [HttpGet("{driverId}")]
        public async Task<IActionResult> GetDriverById([FromRoute] Guid driverId)
        {
            var response = await driverService.GetDriverByIdAsync(driverId);
            if (!response.Success)
                return ToActionResult(response);

            return Ok(response);
        }

        [HttpGet("license/{licenseNumber}")]
        public async Task<IActionResult> GetDriverByLicense([FromRoute] string licenseNumber)
        {
            var response = await driverService.GetDriverByLicenseAsync(licenseNumber);
            if (!response.Success)
                return ToActionResult(response);

            return Ok(response);
        }
    

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

        [HttpPost("end-trip")]
        public async Task<IActionResult> EndTrip()
        {
            var driverIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(driverIdClaim, out Guid driverId))
                return Unauthorized(ApiResponseFactory.Unauthorized());

            var response = await tripService.EndTripAsync(driverId);
            if (!response.Success)
            {
                return ToActionResult(response);
            }
            return Ok(response);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetDriverHistory([FromQuery] DriverHistoryRequest request)
        {
            var driverIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(driverIdClaim, out Guid driverId))
                return Unauthorized(ApiResponseFactory.Unauthorized());

            var response = await tripService.GetDriverHistoryAsync(driverId, request);

            if (!response.Success || response is not ApiResponseWithData<DriverHistoryResponse> result || result.Data is null)
            {
                return ToActionResult(response);
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

            if (response is ApiResponseWithData<DriverResponse> result)
                return Ok(result.Data);
                
            return Ok();
        }

        
        [HttpPost("location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateDriverLocationRequest request)
        {

            var driverIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(driverIdClaim, out Guid driverId))
                return Unauthorized(ApiResponseFactory.Unauthorized());

            var updateResponse = await locationTrackingService.UpdateDriverLocationAsync(
                driverId,
                request.Latitude,
                request.Longitude
            );

            if (!updateResponse.Success)
            {
                return ToActionResult(updateResponse);
            }

            return Ok(updateResponse);
        }

        [HttpGet("location/{driverId}")]
        [AllowAnonymous]
        [TypeFilter(typeof(PassengerOnlyFilter))]
        public async Task<IActionResult> GetDriverLocation(Guid driverId)
        {
            var response = await locationTrackingService.GetDriverLocationAsync(driverId);

            if (!response.Success)
            {
                return ToActionResult(response);
            }
            
            if (response is ApiResponseWithData<RouteResultDTO> result)
                return Ok(result.Data);
                
            return Ok();
        }

    }
}
