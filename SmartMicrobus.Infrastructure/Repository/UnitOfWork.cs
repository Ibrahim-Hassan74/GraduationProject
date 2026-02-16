using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IPhotoRepository PhotoRepository { get; }
        public IDriverRepository DriverRepository { get; }
        public IPassengerRepository PassengerRepository { get; }

      
        public IMicrobusRepository MicrobusRepository { get; }
        public ITripRepository TripRepository { get; }
        public IQueueRepository QueueRepository { get; }
        public IQueueItemRepository QueueItemRepository { get; }
        public IRouteRepository RouteRepository { get; }
        public IStationRepository StationRepository { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            PhotoRepository = new PhotoRepository(_context);
            DriverRepository = new DriverRepository(_context);
            PassengerRepository = new PassengerRepository(_context);

            MicrobusRepository = new MicrobusRepository(_context);
            TripRepository = new TripRepository(_context);
            QueueRepository = new QueueRepository(_context);
            QueueItemRepository = new QueueItemRepository(_context);
            RouteRepository = new RouteRepository(_context);
            StationRepository = new StationRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
