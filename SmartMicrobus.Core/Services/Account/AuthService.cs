using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Security.Claims;

namespace SmartMicrobus.Core.Services.Account
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IJwtService _jwtService;
        public Task<ApiResponse> ConfirmAccountAsync(ConfirmAccountDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ApiResponseFactory.NotFound("User not found");
            
            var deleted = await _userManager.DeleteAsync(user);

            if (!deleted.Succeeded)
                return ApiResponseFactory.Failure("Failed to delete account",400, deleted.Errors.Select(e => e.Description).ToList());

            return ApiResponseFactory.Success("Account deleted successfully");
        }

        public async Task<ApiResponse> DeleteUserPhotoAsync(Guid userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Photo)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ApiResponseFactory.NotFound("User not found.");

            if (user.Photo == null)
                return ApiResponseFactory.BadRequest("User has no photo to delete.");

            _imageService.DeleteImageAsync(user.Photo.ImageName);
            await _unitOfWork.PhotoRepository.DeleteAsync(user.Photo.Id);
            user.Photo = null;

            await _unitOfWork.CompleteAsync();
            await _userManager.UpdateAsync(user);

            return ApiResponseFactory.Success("User photo deleted successfully.");
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

        public async Task<ApiResponse> RefreshTokenAsync(TokenModel model)
        {
            var validationResult = ValidationHelper.ModelValidation(model);
            if (!validationResult.Success)
                return validationResult;

            ClaimsPrincipal? principal;

            try
            {
                principal = _jwtService.GetPrincipalFromJwtToken(model.Token);
            }
            catch (SecurityTokenException ex)
            {
                return ApiResponseFactory.Failure("Invalid token.", 400, "Access token is invalid.");
            }

            if (principal is null)
                return ApiResponseFactory.Failure("Invalid token.", 400, "Access token is invalid.");

            var phone = principal.FindFirstValue(ClaimTypes.MobilePhone);
            if (string.IsNullOrWhiteSpace(phone))
                return ApiResponseFactory.Failure("Invalid token.", 400, "Email claim is missing in token.");

            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.PhoneNumber == phone);
            if (user is null)
                return ApiResponseFactory.Failure("User not found.", 404, "User does not exist.");

            if (user.RefreshToken != model.RefreshToken ||
                user.RefreshTokenExpirationDateTime <= DateTimeOffset.UtcNow)
            {
                return ApiResponseFactory.Failure("Invalid refresh token.", 400, "Refresh token is invalid or expired.");
            }


            bool rememberMe = bool.TryParse(principal.FindFirst("remember_me")?.Value, out var rm) && rm;

            var authResponse = await _jwtService.CreateJwtToken(user, rememberMe) as ApiSuccessResponse;


            // Rotate refresh token
            user.RefreshToken = authResponse?.RefreshToken;
            user.RefreshTokenExpirationDateTime = authResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(user);

            authResponse.Success = true;
            authResponse.StatusCode = 200;
            authResponse.Message = "Token refreshed successfully.";

            return authResponse;
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

        public async Task<ApiResponse> UploadUserPhotoAsync(Guid userId, UploadUserPhotoDTO dto)
        {
            var user = await _userManager.Users
                .Include(u => u.Photo)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ApiResponseFactory.NotFound("User not found.");

            var file = dto.File;
            if (file == null || file.Length == 0)
                return ApiResponseFactory.BadRequest("No file provided.");

            if (!file.ContentType.StartsWith("image/"))
                return ApiResponseFactory.BadRequest("Invalid image type.");

            if (file.Length > 5 * 1024 * 1024)
                return ApiResponseFactory.BadRequest("Image size exceeds limit.");

            if (user.Photo != null)
            {
                _imageService.DeleteImageAsync(user.Photo.ImageName);
                await _unitOfWork.PhotoRepository.DeleteAsync(user.Photo.Id);
                user.Photo = null;
            }

            var folderName = user.PhoneNumber?.Replace(" ", "").ToLowerInvariant();

            var imagePath = await _imageService.SaveUserAvatarAsync(
                file,
                $"Users/{folderName}",
                dto.Crop
            );

            user.Photo = new Photo
            {
                ImageName = imagePath,
                UserId = userId
            };

            await _unitOfWork.CompleteAsync();
            await _userManager.UpdateAsync(user);

            return ApiResponseFactory.Success("User photo uploaded successfully.");
        }
    }
}
