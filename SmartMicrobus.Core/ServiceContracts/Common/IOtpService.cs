using SmartMicrobus.Core.Domain.IdentityEntities;

namespace SmartMicrobus.Core.ServiceContracts.Common
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(ApplicationUser user);
        Task<bool> VerifyOtpAsync(ApplicationUser user, string enteredOtp);
        Task ClearOtpAsync(ApplicationUser user);
    }
}
