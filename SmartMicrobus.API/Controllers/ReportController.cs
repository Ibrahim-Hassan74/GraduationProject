using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.ServiceContracts.Report;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.DTO.Common;
using System.Security.Claims;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Passenger))]
    public class ReportController : CustomControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]

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

        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] UpdateReportRequest request)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var res = await _reportService.UpdateReportAsync(passengerId, id, request);
            return ToActionResult(res);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var res = await _reportService.DeleteReportAsync(passengerId, id);

            return ToActionResult(res);
        }
    }
}
