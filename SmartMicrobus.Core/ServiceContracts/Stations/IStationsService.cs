using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Station;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.ServiceContracts.Stations
{
    public interface IStationsService
    {
        Task<ApiResponse> GetNearestStationAsync(double lat, double lng, TransportMode mode);
        Task<ApiResponse> GetAllStationsAsync();
        Task<ApiResponse> GetStationByIdAsync(Guid id);
        Task<ApiResponse> GetStationDetailsWithRouteAsync(Guid stationId, double userLat, double userLng, TransportMode mode);
        Task<ApiResponse> GetRouteBetweenStationsAsync(Guid fromStationId, Guid toStationId, TransportMode mode);
        Task<ApiResponse> AddStationAsync(StationAddRequest stationAddRequest);
        Task<ApiResponse> UpdateStationAsync(Guid id, StationUpdateRequest stationUpdateRequest);
        Task<ApiResponse> DeleteStationAsync(Guid id);
    }
}
