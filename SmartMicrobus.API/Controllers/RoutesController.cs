using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts.Route;

namespace SmartMicrobus.API.Controllers
{
    public class RoutesController(IRoutesService routeService) : CustomControllerBase
    {
        private readonly IRoutesService _routeService = routeService;
        [HttpGet]
        public async Task<IActionResult> GetAllRoutes()
        {
            var response = await _routeService.GetAllRoutesAsync();
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<List<RouteLocationResponse>>;
            return Ok(result?.Data);
        }
        [HttpGet("destinations")]
        public async Task<IActionResult> GetDestinationsByFrom([FromQuery] string from)
        {
            var response = await _routeService.GetDestinationsByFromAsync(from);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<List<DestinationResponse>>;
            return Ok(result?.Data);
        }

        [HttpGet("{routeId}/summary")]
        public async Task<IActionResult> GetRouteSearchResult(Guid routeId)
        {
            var response = await _routeService.GetRouteSearchResultAsync(routeId);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<RouteSummaryResponse>;
            return Ok(result?.Data);
        }

        [HttpGet("{routeId}/station-microbuses")]
        public async Task<IActionResult> GetMicrobusesAtStation(Guid routeId)
        {
            var response = await _routeService.GetMicrobusesAtStationAsync(routeId);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<List<MicrobusAtStationResponse>>;
            return Ok(result?.Data);
        }

        [HttpGet("{routeId}/on-the-way")]
        public async Task<IActionResult> GetMicrobusesOnTheWay(Guid routeId)
        {
            var response = await _routeService.GetMicrobusesOnTheWayAsync(routeId);
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<List<MicrobusOnTheWayResponse>>;
            return Ok(result?.Data);
        }

        [HttpPost]
        [Route("add-route")]
        [Authorize(nameof(UserRole.Manager))]
        public async Task<IActionResult> AddRoute([FromBody] RouteAddRequest routeAddRequest)
        {
            var response = await _routeService.AddRouteAsync(routeAddRequest);

            if (!response.Success)
                return ToActionResult(response);

            return Ok(response.Message);
        }

        [HttpPatch]
        [Route("update-route")]
        [Authorize(nameof(UserRole.Manager))]
        public async Task<IActionResult> UpdateRoute([FromBody] RouteUpdateRequest routeUpdateRequest)
        {
            var response = await _routeService.UpdateRouteAsync(routeUpdateRequest);

            if (!response.Success)
                return ToActionResult(response);
            return Ok(response.Message);
        }

        [HttpDelete]
        [Route("delete-route")]
        public async Task<IActionResult> DeleteRoute([FromQuery] Guid routeId)
        {
            var response = await _routeService.DeleteRouteAsync(routeId);
            if (!response.Success)
                return ToActionResult(response);
            return Ok(response.Message);
        }
    }
}
