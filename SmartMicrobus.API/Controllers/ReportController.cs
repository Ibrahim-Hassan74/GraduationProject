using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.ServiceContracts.Report;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.DTO.Common;
using System.Security.Claims;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.API.Controllers
{
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
    }
}
