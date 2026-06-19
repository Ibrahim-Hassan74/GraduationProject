using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts.Manager;
using SmartMicrobus.Core.DTO.Microbus;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Station;
using SmartMicrobus.Core.Helper;

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
        [HttpGet("dashboard/overview")]
        public async Task<IActionResult> GetDashboard()
        {
            var stationId = Guid.Parse(User.FindFirst("StationId")?.Value);

            var result = await managerService.GetStationDashboardAsync(stationId);

            if (!result.Success)
                return ToActionResult(result);

            var response = result as ApiResponseWithData<StationDashboardDTO>;

            return Ok(response?.Data);
        }

        [HttpGet]
        [Route("export-station-data")]
        public async Task<IActionResult> ExportStationData([FromQuery] DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid managerId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportStationDataExcelAsync(managerId, startDate, endDate);

            if (!response.Success)
            {
                return ToActionResult(response);
            }

            var fileName = $"StationData_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [Route("export-station-drivers")]
        public async Task<IActionResult> ExportStationDrivers()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid managerId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportStationDriversExcelAsync(managerId);

            if (!response.Success)
            {
                return ToActionResult(response);
            }

            var fileName = $"StationDrivers_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [Route("export-station-routes")]
        public async Task<IActionResult> ExportStationRoutes()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid managerId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportStationRoutesExcelAsync(managerId);

            if (!response.Success)
            {
                return ToActionResult(response);
            }

            var fileName = $"StationRoutes_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [Route("export-station-microbuses")]
        public async Task<IActionResult> ExportMicrobuses()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid managerId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportMicrobusesExcelAsync(managerId);

            if (!response.Success)
            {
                return ToActionResult(response);
            }

            var fileName = $"Microbuses_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [Route("station-microbuses")]
        public async Task<IActionResult> GetPaginatedStationMicrobuses([FromQuery] MicrobusQuery query)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await managerService.GetPaginatedStationMicrobusesAsync(query, stationId);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<Pagination<List<MicrobusResponse>>>;
            return Ok(result?.Data);
        }
    }
}
