namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IUnitOfWork
    {
        IPhotoRepository PhotoRepository { get; }
        Task<int> CompleteAsync();
    }
}
