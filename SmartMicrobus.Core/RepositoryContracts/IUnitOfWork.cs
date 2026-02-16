namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IUnitOfWork
    {
        IPhotoRepository PhotoRepository { get; }
        IDriverRepository DriverRepository { get; }
        IPassengerRepository PassengerRepository { get; }
        IMicrobusRepository MicrobusRepository { get; }
        ITripRepository TripRepository { get; }
        IQueueRepository QueueRepository { get; }
        IQueueItemRepository QueueItemRepository { get; }
        IRouteRepository RouteRepository { get; }
        IStationRepository StationRepository { get; }
        Task<int> CompleteAsync();
    }
}
