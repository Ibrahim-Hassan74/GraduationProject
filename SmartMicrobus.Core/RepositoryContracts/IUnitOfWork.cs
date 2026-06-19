using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IUnitOfWork
    {
        IPhotoRepository PhotoRepository { get; }
        IDriverRepository DriverRepository { get; }
        IPassengerRepository PassengerRepository { get; }
        IManagerRepository ManagerRepository { get; }
        IMicrobusRepository MicrobusRepository { get; }
        ITripRepository TripRepository { get; }
        IQueueRepository QueueRepository { get; }
        IQueueItemRepository QueueItemRepository { get; }
        IRouteRepository RouteRepository { get; }
        IStationRepository StationRepository { get; }
        IFavoriteRouteRepository FavoriteRouteRepository { get; }
        IReportRepository ReportRepository { get; }
        IReportReasonRepository ReportReasonRepository { get; }
        IStaffRepository StaffRepository { get; }

        Task<int> CompleteAsync();

        /// <summary>
        /// Execute an action inside a DB transaction. Implementation should commit on success and rollback on error.
        /// Use this to avoid directly depending on EF Core in the Core project.
        /// </summary>
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
