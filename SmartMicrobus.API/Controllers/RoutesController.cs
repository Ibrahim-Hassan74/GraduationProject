using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.DTO.Common;
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

        [HttpGet("route")]
        public async Task<IActionResult> GetRoute(double lat1, double lng1, double lat2, double lng2)
        {
            var result = await _osrmRouteService.GetRouteAsync(new RouteRequest() { StartLat = lat1, StartLng = lng1, EndLat = lat2, EndLng = lng2 });

            return Ok(result);
        }
    }
}
