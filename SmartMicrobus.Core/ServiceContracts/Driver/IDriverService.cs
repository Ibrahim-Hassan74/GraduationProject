using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.ServiceContracts.Driver
{
    public interface IDriverService
    {
        Task<DriverDashboardDTO> GetDashboardAsync(Guid driverId);

        Task<Guid> EnterQueueAsync(string qrCode, Guid stationId);

        Task LeaveQueueAsync(Guid driverId);

        Task<Guid> StartTripAsync(Guid driverId);

        Task EndTripAsync(Guid driverId);

        Task<List<QueueItemDTO>> GetMyQueueAsync(Guid driverId);
    }

}
