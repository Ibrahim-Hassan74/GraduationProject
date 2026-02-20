using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Staff
{
    public interface IStaffService
    {
        Task<ApiResponse> CheckInAtGateAsync(string qrCode, Guid stationId);

        Task<ApiResponse> CheckOutAtGateAsync(string qrCode);
    }
}
