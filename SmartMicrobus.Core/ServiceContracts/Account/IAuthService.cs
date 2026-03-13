using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Account
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterDriverAsync(RegisterDriverDTO dto);
        Task<ApiResponse> RegisterPassengerAsync(RegisterPassengerDTO dto);
        Task<ApiResponse> LoginAsync(LoginDTO loginDTO);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO dto);
        Task<ApiResponse> VerifyOtpAsync(VerifyOtpDTO dto);
        Task<ApiResponse> ConfirmAccountAsync(ConfirmAccountDTO dto);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<ApiResponse> RefreshTokenAsync(TokenModel model);
        Task<ApiResponse> LogoutAsync(Guid userId);
        Task<ApiResponse> UpdateUserProfileAsync(UpdateUserDTO dto);
        Task<ApiResponse> ExternalLoginCallbackAsync(string remoteError = "");
        Task<ApiResponse> DeleteAccountAsync(string userId);
        Task<ApiResponse> ResendConfirmationAccountAsync(string phone);
        Task<ApiResponse> UploadUserPhotoAsync(Guid userId, UploadUserPhotoDTO dto);
        Task<ApiResponse> DeleteUserPhotoAsync(Guid userId);
        Task<ApplicationUserResponse> GetUserByIdAsync(string userId);
    }
}
