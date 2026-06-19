namespace SmartMicrobus.Core.ServiceContracts.Route
{
    public interface IRouteTrackingNotificationService
    {
        /// <summary>
        /// Called on check-in/check-out events (queue join, trip start, trip end).
        /// Re-queries counts and broadcasts to route subscribers.
        /// </summary>
        Task NotifyRouteUpdated(Guid routeId);

        /// <summary>
        /// Called from LocationTrackingService on every GPS update.
        /// Tracks per-driver ETA and broadcasts the minimum to route subscribers.
        /// </summary>
        Task UpdateDriverEtaAsync(Guid routeId, Guid driverId, int etaMinutes);

        /// <summary>
        /// Called when a driver's trip ends — removes their ETA from tracking.
        /// </summary>
        void RemoveDriverEta(Guid routeId, Guid driverId);
    }
}
