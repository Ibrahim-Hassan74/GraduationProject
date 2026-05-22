using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;

namespace SmartMicrobus.Core.ServiceContracts.Route
{
    public interface IRoutesService
    {
        Task<ApiResponse> GetAllRoutesAsync(); // return List<RouteLocationResponse>

        Task<ApiResponse> GetDestinationsByFromAsync(Guid fromStationId); // return List<DestinationResponse> of destinations

        Task<ApiResponse> GetRouteSearchResultAsync(Guid routeId); // return RouteSummaryResponse

        Task<ApiResponse> GetMicrobusesAtStationAsync(Guid routeId); // return List<MicrobusAtStationResponse> (includes microbus details)

        Task<ApiResponse> GetMicrobusesOnTheWayAsync(Guid routeId); // return List<MicrobusOnTheWayResponse> (includes microbus details and estimated arrival time)

        Task<ApiResponse> AddRouteAsync(RouteAddRequest routeAddRequest); 

        Task<ApiResponse> UpdateRouteAsync(RouteUpdateRequest routeUpdateRequest);
        Task<ApiResponse> DeleteRouteAsync(Guid routeId);
    }
}
