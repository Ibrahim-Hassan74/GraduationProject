using SmartMicrobus.Core.DTO.Route;

namespace SmartMicrobus.Core.ServiceContracts.Route
{
    public interface IOsrmRouteService
    {
        Task<RouteResult> GetRouteAsync(RouteRequest request);
    }
}
