using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.ServiceContracts.Driver
{
    public interface IDriverService
    {
        Task<Guid> CheckInAtGateAsync(string qrCode, Guid stationId);

        Task<bool> CheckOutAtGateAsync(string qrCode);

        Task<DriverDashboardDTO> GetDashboardAsync(Guid driverId);

        Task<List<QueueItemDTO>> GetDriversBeforeMeAsync(Guid driverId);

        Task<List<QueueItemDTO>> GetQueueByRouteAsync(Guid stationId, Guid routeId);

        Task<Guid> StartTripAsync(Guid driverId);

        Task<bool> EndTripAsync(Guid driverId);

        Task<bool> ResetDailyQueueAsync();
    }

}
