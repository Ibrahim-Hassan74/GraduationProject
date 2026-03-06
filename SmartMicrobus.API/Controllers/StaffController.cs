using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts.Staff;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Staff))]
    public class StaffController(ILogger<StaffController> logger, IStaffService staffService) : CustomControllerBase
    {
        private readonly IStaffService _staffService = staffService;
        private readonly ILogger<StaffController> _logger = logger;
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckInAtGate([FromBody] CheckInRequest request)
        {
            var response = await _staffService.CheckInAtGateAsync(request.QrCode, request.StationId);
            return ToActionResult(response);
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOutAtGate([FromBody] CheckOutRequest request)
        {
            var response = await _staffService.CheckOutAtGateAsync(request.QrCode);

            return ToActionResult(response);
        }

    }
}
