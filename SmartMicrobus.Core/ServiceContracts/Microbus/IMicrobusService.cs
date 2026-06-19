using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Microbus;

namespace SmartMicrobus.Core.ServiceContracts.Microbus
{
    public interface IMicrobusService
    {
        Task<ApiResponse> GetPaginatedMicrobusesAsync(Guid stationId, MicrobusQuery query);
        Task<ApiResponse> GetMicrobusByIdAsync(Guid microbusId, Guid stationId);
    }
}
