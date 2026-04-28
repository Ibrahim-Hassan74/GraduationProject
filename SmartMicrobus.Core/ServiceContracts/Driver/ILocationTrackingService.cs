using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;

namespace SmartMicrobus.Core.ServiceContracts.Driver
{
    /// <summary>
    /// Service contract for managing real-time driver location tracking.
    /// </summary>
    public interface ILocationTrackingService
    {
        /// <summary>
        /// Updates the driver's current location.
        /// </summary>
        Task<ApiResponse> UpdateDriverLocationAsync(Guid driverId, double latitude, double longitude);

        /// <summary>
        /// Retrieves the last known location of a driver.
        /// </summary>
       Task<ApiResponse> GetDriverLocationAsync(Guid driverId);

    
    }
}
