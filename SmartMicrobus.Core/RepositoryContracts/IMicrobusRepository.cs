using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IMicrobusRepository : IGenericRepository<Microbus>
    {
        Task<Microbus?> GetByQrCodeAsync(string qrCode);
    }
}