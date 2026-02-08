namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IUnitOfWork
    {
        IPhotoRepository PhotoRepository { get; }
        IDriverRepository DriverRepository { get; }
        IPassengerRepository PassengerRepository { get; }
        Task<int> CompleteAsync();
    }
}
