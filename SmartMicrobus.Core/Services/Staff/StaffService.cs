using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.ServiceContracts.Staff;

namespace SmartMicrobus.Core.Services.Staff
{
    public class StaffService : IStaffService
    {

        public Task<ApiResponse> CheckInAtGateAsync(string qrCode, Guid stationId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> CheckOutAtGateAsync(string qrCode)
        {
            throw new NotImplementedException();
        }
    }
}
