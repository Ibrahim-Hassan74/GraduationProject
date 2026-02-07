using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.Services.Common;
using System.Security.Cryptography;

namespace SmartMicrobus.Core.Services.Account
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IWhatsAppService _whatsAppService;

        private const string OtpLoginProvider = "OTP";
        private const string OtpTokenName = "ForgotPassword";

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IWhatsAppService whatsAppService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _whatsAppService = whatsAppService;
        }

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

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return ApiResponseFactory.BadRequest("Phone number is required.");

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (user is null)
            {
                return ApiResponseFactory.Success("If an account with the provided phone number exists, an OTP has been sent.");
            }

            int num = RandomNumberGenerator.GetInt32(0, 1_000_000);
            var otp = num.ToString("D6");
            var expiry = DateTimeOffset.UtcNow.AddMinutes(5);
            var storedValue = $"{otp}|{expiry.ToUnixTimeSeconds()}";

            await _userManager.SetAuthenticationTokenAsync(user, OtpLoginProvider, OtpTokenName, storedValue);

            try
            {
                await _whatsAppService.SendInvoiceMessageAsync(user.PhoneNumber!, otp);
            }
            catch
            {
                return ApiResponseFactory.InternalServerError("SomeThing error try agian letter.");

            }

            return ApiResponseFactory.Success("If an account with the provided phone number exists, an OTP has been sent.");
        }

        public async Task<ApiResponse> VerifyOtpAsync(VerifyOtpDTO dto)
        {
            if (dto is null)
                return ApiResponseFactory.BadRequest("Data is required.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber) || string.IsNullOrWhiteSpace(dto.Otp))
                return ApiResponseFactory.BadRequest("Phone number and OTP are required.");

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (user is null)
                return ApiResponseFactory.BadRequest("Invalid phone number or OTP.");

            var stored = await _userManager.GetAuthenticationTokenAsync(user, OtpLoginProvider, OtpTokenName);
            if (string.IsNullOrWhiteSpace(stored))
                return ApiResponseFactory.BadRequest("No OTP found or it has expired.");

            var parts = stored.Split('|');
            if (parts.Length != 2)
                return ApiResponseFactory.BadRequest("Invalid OTP stored value.");

            var storedOtp = parts[0];
            if (!long.TryParse(parts[1], out var expirySeconds))
                return ApiResponseFactory.BadRequest("Invalid OTP expiry.");

            var expiryTime = DateTimeOffset.FromUnixTimeSeconds(expirySeconds);
            if (DateTimeOffset.UtcNow > expiryTime)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, OtpLoginProvider, OtpTokenName);
                return ApiResponseFactory.BadRequest("OTP has expired.");
            }

            if (!string.Equals(storedOtp, dto.Otp, StringComparison.Ordinal))
                return ApiResponseFactory.BadRequest("Invalid OTP.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _userManager.RemoveAuthenticationTokenAsync(user, OtpLoginProvider, OtpTokenName);

            return ApiResponseFactory.Success("OTP verified successfully.",new { resetToken, user.Id });
        }

        public async Task<ApiResponse> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO is null)
                return ApiResponseFactory.BadRequest("Login data is required.");

            if (string.IsNullOrWhiteSpace(loginDTO.PhoneNumber) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return ApiResponseFactory.BadRequest("Phone number and password are required.");
            }


            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == loginDTO.PhoneNumber);

            if (user is null)
            {
                return ApiResponseFactory.Unauthorized("Invalid phone number or password.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                return ApiResponseFactory.Forbidden("User account is locked. Please try again later or contact support.");
            }

            if (signInResult.IsNotAllowed)
            {
                return ApiResponseFactory.Forbidden("User is not allowed to sign in.");
            }


            if (!signInResult.Succeeded)
            {
                return ApiResponseFactory.Unauthorized("Invalid phone number or password.");
            }

            var jwtResponse = await _jwtService.CreateJwtToken(user, loginDTO.RememberMe);

            return jwtResponse;
        }

        public async Task<ApiResponse> LogoutAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ApiResponseFactory.BadRequest("UserId is required.");

            await _signInManager.SignOutAsync();

            return ApiResponseFactory.Success("Logged out successfully.");
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

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto is null)
                return ApiResponseFactory.BadRequest("Reset data is required.");

            if (dto.UserId == Guid.Empty || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return ApiResponseFactory.BadRequest("UserId, token and new password are required.");

            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user is null)
                return ApiResponseFactory.NotFound("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponseFactory.BadRequest("Failed to reset password.", errors);
            }

            return ApiResponseFactory.Success("Password has been reset successfully.");
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
