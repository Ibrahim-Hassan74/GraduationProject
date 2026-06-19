using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Microbus;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts.Microbus;
using SmartMicrobus.Core.Services.Manager;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Manager))]
    public class MicrobusController : CustomControllerBase
    {
        private readonly IMicrobusService _microbusService;

        public MicrobusController(IMicrobusService microbusService)
        {
            _microbusService = microbusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMicrobuses([FromQuery] MicrobusQuery query)
        {
            var stationId = Guid.Parse(
                User.FindFirst("StationId")!.Value);

            var result = await _microbusService.GetPaginatedMicrobusesAsync(stationId, query);

            if (!result.Success)
                return ToActionResult(result);

            var response = result as ApiResponseWithData<PagedResponse<MicrobusListResponse>>;

            return Ok(response?.Data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMicrobusById(Guid id)
        {
            var stationId = Guid.Parse(
                User.FindFirst("StationId")!.Value);

            var result = await _microbusService.GetMicrobusByIdAsync(id, stationId);

            if (!result.Success)
                return ToActionResult(result);

            var response = result as ApiResponseWithData<MicrobusDetailsResponse>;

            return Ok(response?.Data);
        }
    }
}
