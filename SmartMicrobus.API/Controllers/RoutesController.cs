using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Route;

namespace SmartMicrobus.API.Controllers
{
    public class RoutesController(IRoutesService routeService, IOsrmRouteService osrmRouteService) : CustomControllerBase
    {
        private readonly IRoutesService _routeService = routeService;
        private readonly IOsrmRouteService _osrmRouteService = osrmRouteService;
        [HttpGet]
        public async Task<IActionResult> GetAllRoutes()
        {
            var response = await _routeService.GetAllRoutesAsync();
            if (!response.Success)
                return ToActionResult(response);

            var result = response as ApiResponseWithData<List<RouteLocationResponse>>;
            return Ok(result?.Data);
        }

        [HttpGet("all")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> GetPaginated([FromQuery] RouteQuery query)
        {
            var stationId = Guid.Parse(User.FindFirst("StationId")?.Value);
            var response = await _routeService.GetPaginatedRoutesAsync(query, stationId);
            if (!response.Success)
                return ToActionResult(response);
            var result = response as ApiResponseWithData<Pagination<List<RouteDetails>>>;

            return Ok(result?.Data);
        }
        [HttpGet("destinations")]
        public async Task<IActionResult> GetDestinationsByFrom([FromQuery] Guid fromStationId)
        {
            var response = await _routeService.GetDestinationsByFromAsync(fromStationId);
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
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> AddRoute([FromBody] RouteAddRequest routeAddRequest)
        {
            var stationIdValue = User.FindFirst("StationId")?.Value;
            if (string.IsNullOrEmpty(stationIdValue) || !Guid.TryParse(stationIdValue, out Guid stationId))
            {
                return Unauthorized(new { Message = "Station ID not found or invalid." });
            }

            var response = await _routeService.AddRouteAsync(stationId, routeAddRequest);

            if (!response.Success)
                return ToActionResult(response);

            return Ok(response.Message);
        }

        [HttpPatch]
        [Route("update-route")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> UpdateRoute([FromBody] RouteUpdateRequest routeUpdateRequest)
        {
            var response = await _routeService.UpdateRouteAsync(routeUpdateRequest);

            if (!response.Success)
                return ToActionResult(response);
            return Ok(response.Message);
        }

        [HttpDelete]
        [Route("delete-route")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> DeleteRoute([FromQuery] Guid routeId)
        {
            var response = await _routeService.DeleteRouteAsync(routeId);
            if (!response.Success)
                return ToActionResult(response);
            return Ok(response.Message);
        }    
        [HttpGet("route")]
        public async Task<IActionResult> GetRoute(double lat1, double lng1, double lat2, double lng2)
        {
            var result = await _osrmRouteService.GetRouteAsync(new RouteRequest() { StartLat = lat1, StartLng = lng1, EndLat = lat2, EndLng = lng2 });

            return Ok(result);
        }
        [HttpGet("{routeId:guid}")]
        [Authorize(Roles = nameof(UserRole.Manager))]
        public async Task<IActionResult> GetRouteById([FromRoute]Guid routeId)
        {
            var stationId = Guid.Parse(User.FindFirst("StationId")?.Value);
            var result = await _routeService.GetRouteByIdAsync(routeId, stationId);

            if (!result.Success)
                return ToActionResult(result);
            var response = result as ApiResponseWithData<RouteDetails>;

            return Ok(response?.Data);
        }
    }
}
