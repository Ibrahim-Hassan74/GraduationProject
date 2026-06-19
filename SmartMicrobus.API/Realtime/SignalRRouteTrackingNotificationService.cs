using Microsoft.AspNetCore.SignalR;
using SmartMicrobus.API.Hubs;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Route;
using System.Collections.Concurrent;

namespace SmartMicrobus.API.Realtime
{
    public class SignalRRouteTrackingNotificationService
    : IRouteTrackingNotificationService
    {
        private readonly IHubContext<RouteTrackingHub> _hub;
        private readonly ITripRepository _tripRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueueItemRepository _queueRepository;

        // Static so ETA state persists across scoped service instances.
        // Key: routeId → { driverId → etaMinutes }
        private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, int>> _routeDriverEtas = new();

        public SignalRRouteTrackingNotificationService(
            IHubContext<RouteTrackingHub> hub,
            IUnitOfWork unitOfWork)
        {
            _hub = hub;
            _unitOfWork = unitOfWork;
            _tripRepository = _unitOfWork.TripRepository;
            _queueRepository = _unitOfWork.QueueItemRepository;
        }

        /// <summary>
        /// Called on check-in/check-out events — re-queries counts and includes
        /// the best known ETA from the in-memory tracker.
        /// </summary>
        public async Task NotifyRouteUpdated(Guid routeId)
        {
            var onTheWay =
                await _tripRepository.GetMicrobusesOnTheWayCountAsync(routeId);

            var inQueue =
                await _queueRepository.GetMicrobusesAtStationCountAsync(routeId);

            int? nearestEta = null;

            if (inQueue > 0)
            {
                nearestEta = 0;
            }
            else if (_routeDriverEtas.TryGetValue(routeId, out var driverEtas) && !driverEtas.IsEmpty)
            {
                nearestEta = driverEtas.Values.Min();
            }

            var dto = new RouteLiveUpdateDTO
            {
                NumberOfMicrobusesInQueue = inQueue,
                NumberOfMicrobusesOnTheWay = onTheWay,
                NearestArrivalMinutes = nearestEta
            };

            await _hub.Clients
                .Group($"route-{routeId}")
                .SendAsync("RouteUpdated", dto);
        }

        /// <summary>
        /// Called from LocationTrackingService on every GPS update.
        /// Tracks the ETA per driver and broadcasts the minimum across all active drivers.
        /// </summary>
        public async Task UpdateDriverEtaAsync(Guid routeId, Guid driverId, int etaMinutes)
        {
            var driverEtas = _routeDriverEtas.GetOrAdd(routeId, _ => new ConcurrentDictionary<Guid, int>());
            driverEtas[driverId] = etaMinutes;

            var minEta = driverEtas.Values.Min();

            var onTheWay =
                await _tripRepository.GetMicrobusesOnTheWayCountAsync(routeId);

            var inQueue =
                await _queueRepository.GetMicrobusesAtStationCountAsync(routeId);

            var dto = new RouteLiveUpdateDTO
            {
                NumberOfMicrobusesInQueue = inQueue,
                NumberOfMicrobusesOnTheWay = onTheWay,
                NearestArrivalMinutes = inQueue > 0 ? 0 : minEta
            };

            await _hub.Clients
                .Group($"route-{routeId}")
                .SendAsync("RouteUpdated", dto);
        }

        /// <summary>
        /// Removes a driver's ETA entry when their trip ends.
        /// </summary>
        public void RemoveDriverEta(Guid routeId, Guid driverId)
        {
            if (_routeDriverEtas.TryGetValue(routeId, out var driverEtas))
            {
                driverEtas.TryRemove(driverId, out _);

                // Clean up empty route entries
                if (driverEtas.IsEmpty)
                    _routeDriverEtas.TryRemove(routeId, out _);
            }
        }
    }
}
