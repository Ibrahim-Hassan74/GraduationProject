using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Station;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Stations;

namespace SmartMicrobus.API.Controllers
{
    public class StationsController(IStationsService stationsService, IStringLocalizer<SharedResource> localizer) : CustomControllerBase
    {
        private readonly IStationsService _stationsService = stationsService;
        private readonly IStringLocalizer<SharedResource> _localizer = localizer;

        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearestStation(double lat, double lng, TransportMode mode = TransportMode.Driving)
        {
            if (!GeoValidator.IsValid(lat, lng))
                return BadRequest(ApiResponseFactory.BadRequest(_localizer["InvalidCoordinates"]));

            var result = await _stationsService.GetNearestStationAsync(lat, lng, mode);
            if(!result.Success)
                return ToActionResult(result);
            var reponse = result as ApiResponseWithData<NearestStationResponse>;
            return Ok(reponse?.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _stationsService.GetAllStationsAsync();
            if(!result.Success)
                return ToActionResult(result);
            var response = result as ApiResponseWithData<IEnumerable<StationResponse>>;
            return Ok(response?.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _stationsService.GetStationByIdAsync(id);
            if(!result.Success)
                return ToActionResult(result);
            var response = result as ApiResponseWithData<StationResponse>;
            return Ok(response?.Data);
        }

        [HttpGet("{id}/details-with-route")]
        public async Task<IActionResult> GetStationDetailsWithRoute(Guid id, double lat, double lng, TransportMode mode = TransportMode.Driving)
        {
            if(!GeoValidator.IsValid(lat, lng))
                return BadRequest(ApiResponseFactory.BadRequest(_localizer["InvalidCoordinates"]));

            var result = await _stationsService.GetStationDetailsWithRouteAsync(id, lat, lng, mode);
            if(!result.Success)
                return ToActionResult(result);
            var response = result as ApiResponseWithData<StationDetailsWithRouteResponse>;

            return Ok(response?.Data);
        }

        [HttpGet("route-between")]
        public async Task<IActionResult> GetRouteBetweenStations(Guid fromStationId, Guid toStationId, TransportMode mode = TransportMode.Driving)
        {
            var result = await _stationsService.GetRouteBetweenStationsAsync(fromStationId, toStationId, mode);

            if (!result.Success)
                return ToActionResult(result);
            var response = result as ApiResponseWithData<RouteBetweenStationsResponse>;
            return Ok(response?.Data);
        }
    }
}
