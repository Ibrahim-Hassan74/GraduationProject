using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Microbus;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IMicrobusRepository : IGenericRepository<Microbus>
    {
        Task<Microbus?> GetByQrCodeAsync(string qrCode);
        Task<Driver?> GetDriverAsync(string plateNumber);

        Task<List<Microbus>> GetAllStationMicrobusesAsync(Guid stationId);
        Task<List<Microbus>> GetActiveMicrobusesByStationAsync(Guid stationId);
        Task<(List<Microbus> Microbuses, int TotalCount)> GetPaginatedByStationAsync(Guid stationId, MicrobusQuery query);
    }
}