using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.ServiceContracts.Account;

namespace SmartMicrobus.Core.Services.Account
{
    public class AuthService : IAuthService
    {
        public Task<ApiResponse> ConfirmAccountAsync(ConfirmAccountDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeleteAccountAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeleteUserPhotoAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> ExternalLoginCallbackAsync(string remoteError = "")
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> LoginAsync(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> LogoutAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> RefreshTokenAsync(TokenModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> RegisterDriverAsync(RegisterDriverDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> RegisterPassengerAsync(RegisterPassengerDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> ResendConfirmationAccountAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UpdateUserProfileAsync(UpdateUserDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UploadUserPhotoAsync(Guid userId, UploadUserPhotoDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
