using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Microbus;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.DTO.Staff;
using SmartMicrobus.Core.DTO.Station;
using SmartMicrobus.Core.ServiceContracts.Manager;
using SmartMicrobus.Core.ServiceContracts.Staff;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Driver;
using SmartMicrobus.Core.ServiceContracts.Manager;
using System.Security.Claims;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Manager))]
    public class ManagerController(IManagerService managerService, IStaffService staffService, ITripService tripService) : CustomControllerBase
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

        [HttpPut]
        [Route("update-microbus/{microbusId}")]
        public async Task<IActionResult> UpdateMicrobus([FromRoute] Guid microbusId, [FromBody] MicrobusUpdateRequest microbusUpdateRequest)
        {
            if (microbusUpdateRequest == null)
                return BadRequest("microbus data cannot be empty");

            var result = await managerService.UpdateMicrobusAsync(microbusId, microbusUpdateRequest);

            if (!result.Success)
                return ToActionResult(result);

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete-microbus/{microbusId}")]
        public async Task<IActionResult> DeleteMicrobus([FromRoute] Guid microbusId)
        {
            var result = await managerService.DeleteMicrobusAsync(microbusId);

            if (!result.Success)
                return ToActionResult(result);

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
            var stationString = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationString) || !Guid.TryParse(stationString, out Guid stationId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

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
            var stationString = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationString) || !Guid.TryParse(stationString, out Guid stationId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportStationDataExcelAsync(stationId, startDate, endDate);

            if (!response.Success)
            {
                return ToActionResult(response);
            }

            var fileName = $"StationData_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckInAtGate([FromBody] CheckInRequest request)
        {
            var stationId = Guid.Parse(User.FindFirst("StationId")?.Value);
            var response = await staffService.CheckInAtGateAsync(request.QrCode, stationId);
            return ToActionResult(response);
        }



        [HttpGet]
        [Route("export-station-drivers")]
        public async Task<IActionResult> ExportStationDrivers()
        {
            var stationString = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationString) || !Guid.TryParse(stationString, out Guid stationId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportStationDriversExcelAsync(stationId);

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
            var stationString = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationString) || !Guid.TryParse(stationString, out Guid stationId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportStationRoutesExcelAsync(stationId);

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
            var stationString = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationString) || !Guid.TryParse(stationString, out Guid stationId))
            {
                return Unauthorized(new { Message = "User ID not found or invalid." });
            }

            var response = await managerService.ExportMicrobusesExcelAsync(stationId);

            if (!response.Success)
            {
                return ToActionResult(response);
            }

            var fileName = $"Microbuses_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [Route("export-reports")]
        public async Task<IActionResult> ExportReports([FromQuery] GetReportsQuery query)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await managerService.ExportReportsExcelAsync(query, stationId);
            if (!response.Success)
            {
                return ToActionResult(response);
            }

            var fileContent = ((ApiResponseWithData<byte[]>)response).Data;
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Reports_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        //[HttpGet]
        //[Route("station-microbuses")]
        //public async Task<IActionResult> GetPaginatedStationMicrobuses([FromQuery] MicrobusQuery query)
        //{
        //    var stationIdValue = User.FindFirst("StationId")?.Value;
        //    if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
        //    {
        //        return Unauthorized(new { Message = "Station ID not found or invalid." });
        //    }

        //    var response = await managerService.GetPaginatedStationMicrobusesAsync(query, stationId);
        //    if (!response.Success)
        //        return ToActionResult(response);

        //    var result = response as ApiResponseWithData<Pagination<List<MicrobusResponse>>>;
        //    return Ok(result?.Data);
        //}

        [HttpGet]
        [Route("station-drivers")]
        public async Task<IActionResult> GetPaginatedStationDrivers([FromQuery] DriverQuery query)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await managerService.GetPaginatedStationDriversAsync(query, stationId);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<Pagination<List<DriverResponse>>>;
            return Ok(result?.Data);
        }

        [HttpPost]
        [Route("station-staff")]
        public async Task<IActionResult> AddStaff([FromBody] AddStaffDTO dto)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await managerService.AddStaffAsync(dto, stationId);
            return ToActionResult(response);
        }

        [HttpPut]
        [Route("station-staff/{id}")]
        public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffDTO dto)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await managerService.UpdateStaffAsync(id, dto, stationId);
            return ToActionResult(response);
        }

        [HttpDelete]
        [Route("station-staff/{id}")]
        public async Task<IActionResult> DeleteStaff(Guid id)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await managerService.DeleteStaffAsync(id, stationId);
            return ToActionResult(response);
        }

        [HttpGet]
        [Route("station-staff")]
        public async Task<IActionResult> GetPaginatedStationStaff([FromQuery] StaffQuery query)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await managerService.GetPaginatedStationStaffAsync(query, stationId);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<Pagination<List<StaffResponseDTO>>>;
            return Ok(result?.Data);
        }

        [HttpGet("{driverId}/driver-history")]
        public async Task<IActionResult> GetDriverHistory(Guid driverId, [FromQuery] DriverHistoryRequest request)
        {

            var response = await tripService.GetDriverHistoryAsync(driverId, request);

            if (!response.Success || response is not ApiResponseWithData<DriverHistoryResponse> result || result.Data is null)
            {
                return ToActionResult(response);
            }

            var pagination = new Pagination<DriverHistoryResponse>(request.PageNumber, request.PageSize, result.Data.TotalCount, result.Data);

            return Ok(pagination);
        }
    }
}
