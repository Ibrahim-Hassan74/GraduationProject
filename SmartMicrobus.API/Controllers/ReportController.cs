using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.ServiceContracts.Report;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.DTO.Common;
using System.Security.Claims;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.API.Controllers
{
    [Authorize]
    public class ReportController : CustomControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Passenger))]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var res = await _reportService.CreateReportAsync(passengerId, request);

            return ToActionResult(res);
        }

        [HttpGet("reasons")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReasons()
        {
            var res = await _reportService.GetReasonsAsync();

            if (!res.Success)
            {
                return ToActionResult(res);
            }
            var response = res as ApiResponseWithData<List<ReportReasonResponse>>;
            return Ok(response?.Data);
        }

        [HttpGet]

        [Authorize(Roles = nameof(UserRole.Passenger))]
        public async Task<IActionResult> GetReports([FromQuery] GetReportsQuery query)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var res = await _reportService.GetReportsAsync(passengerId, query);


            if (!res.Success)
            {
                return ToActionResult(res);
            }
            var response = res as ApiResponseWithData<PagedResponse<ReportResponse>>;
            return Ok(response?.Data);

        }


        [HttpGet("{id}")]
        [Authorize(Roles = nameof(UserRole.Passenger))]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var res = await _reportService.GetReportByIdAsync(passengerId, id);

            if (!res.Success)
            {
                return ToActionResult(res);
            }
            var response = res as ApiResponseWithData<ReportResponseWithDetails>;
            return Ok(response?.Data);

        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserRole.Passenger))]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] UpdateReportRequest request)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var res = await _reportService.UpdateReportAsync(passengerId, id, request);
            return ToActionResult(res);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Passenger))]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var res = await _reportService.DeleteReportAsync(passengerId, id);

            return ToActionResult(res);
        }

        // Admin / Manager endpoints
        [HttpGet("admin/all")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> GetAllReports([FromQuery] GetReportsQuery query)
        {
            var stationId = Guid.Parse(User.FindFirst("StationId")?.Value);
            var res = await _reportService.GetAllReportsAsync(query, stationId);

            if (!res.Success)
            {
                return ToActionResult(res);
            }
            var response = res as ApiResponseWithData<PagedResponse<ReportResponse>>;
            return Ok(response?.Data);
        }

        [HttpGet("admin/{id}")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> GetReportByIdForAdmin(Guid id)
        {

            var stationId = Guid.Parse(User.FindFirst("StationId")?.Value);
            var res = await _reportService.GetReportByIdForAdminAsync(id, stationId);

            if (!res.Success)
            {
                return ToActionResult(res);
            }
            var response = res as ApiResponseWithData<ReportResponseForManager>;
            return Ok(response?.Data);
        }

        [HttpPatch("admin/{id}/status")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> UpdateReportStatus(Guid id, [FromBody] UpdateReportStatusRequest request)
        {
            var res = await _reportService.UpdateReportStatusAsync(id, request);
            return ToActionResult(res);
        }
    }
}
