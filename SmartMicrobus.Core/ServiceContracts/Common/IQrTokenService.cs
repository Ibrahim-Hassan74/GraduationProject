using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Common
{
    public interface IQrTokenService
    {
        string GenerateToken(MicrobusQrPayload payload);
        MicrobusQrPayload? DecryptToken(string token);
    }
}
