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

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            PhotoRepository = new PhotoRepository(_context);
            DriverRepository = new DriverRepository(_context);
            PassengerRepository = new PassengerRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
