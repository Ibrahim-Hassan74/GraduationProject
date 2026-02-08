using Microsoft.AspNetCore.Identity;
<<<<<<< HEAD
=======
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
>>>>>>> e65c010ef2fca80349078645d5d9912eb28ef042
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;
<<<<<<< HEAD
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Account;
=======
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Security.Claims;
using System.Security.Cryptography;
>>>>>>> e65c010ef2fca80349078645d5d9912eb28ef042

namespace SmartMicrobus.Core.Services.Account
{
    public class AuthService(UserManager<ApplicationUser> userManager, IDriverRepository driverRepository,
        IPassangerRepository passangerRepository) : IAuthService
    {
<<<<<<< HEAD
        public Task<ApiResponse> ConfirmAccountAsync(ConfirmAccountDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeleteAccountAsync(string userId)
=======
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        private const string OtpLoginProvider = "OTP";
        private const string OtpTokenName = "ForgotPassword";

        public AuthService(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IWhatsAppService whatsAppService, 
            IUnitOfWork unitOfWork,
            IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _whatsAppService = whatsAppService;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public Task<ApiResponse> ConfirmAccountAsync(ConfirmAccountDTO dto)
>>>>>>> e65c010ef2fca80349078645d5d9912eb28ef042
        {
            throw new NotImplementedException();
        }

<<<<<<< HEAD
        public Task<ApiResponse> DeleteUserPhotoAsync(Guid userId)
        {
            throw new NotImplementedException();
=======
        public async Task<ApiResponse> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ApiResponseFactory.NotFound("User not found");
            
            var deleted = await _userManager.DeleteAsync(user);

            if (!deleted.Succeeded)
                return ApiResponseFactory.Failure("Failed to delete account",400, deleted.Errors.Select(e => e.Description).ToArray());

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
>>>>>>> e65c010ef2fca80349078645d5d9912eb28ef042
        }

        public Task<ApiResponse> ExternalLoginCallbackAsync(string remoteError = "")
        {
            throw new NotImplementedException();
        }

<<<<<<< HEAD
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

        public async Task<ApiResponse> RegisterDriverAsync(RegisterDriverDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.PhoneNumber,
                PhoneNumber = dto.PhoneNumber,
                DisplayName = dto.DisplayName,
                Role = Enums.UserRole.Driver
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Failed to register driver.",
                    StatusCode = 400 
                };
            }

            //await userManager.AddToRoleAsync(user, user.Role.ToString());

            var driver = new Driver
            {
                driverId = user.Id,
                LicenseNumber = dto.LicenseNumber
            };
            
            await driverRepository.AddDriverAsync(driver);
            return new ApiResponse
            {
                Success = true,
                Message = "Driver registered successfully.",
                StatusCode = 201 
            };
        }

        public async Task<ApiResponse> RegisterPassengerAsync(RegisterPassengerDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.PhoneNumber,
                PhoneNumber = dto.PhoneNumber,
                DisplayName = dto.Name,
                Role = Enums.UserRole.Passenger
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Failed to register passenger.",
                    StatusCode = 400 
                };
            }

            //await userManager.AddToRoleAsync(user, user.Role.ToString());

            var passenger = new Passenger
            {
                PassengerId = user.Id,
            };
            var response = await passangerRepository.AddPassengerAsync(passenger);

            return new ApiResponse
            {
                Success = true,
                Message = "Passenger registered successfully.",
                StatusCode = 201 
            };
=======
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
                await _whatsAppService.SendOTPAsync(user.PhoneNumber!, otp);
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

            var validationResult = ValidationHelper.ModelValidation(dto);
            if (!validationResult.Success)
                return validationResult;

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

            var validationResult = ValidationHelper.ModelValidation(loginDTO);
            if (!validationResult.Success)
                return validationResult;


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

            var jwtResponse = await _jwtService.CreateJwtToken(user, loginDTO.RememberMe) as ApiSuccessResponse;

            user.RefreshToken = jwtResponse?.RefreshToken;
            user.RefreshTokenExpirationDateTime = jwtResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(user);

            return jwtResponse;
        }

        public async Task<ApiResponse> LogoutAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ApiResponseFactory.BadRequest("UserId is required.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpirationDateTime = DateTimeOffset.MinValue;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignOutAsync();

            return ApiResponseFactory.Success("Logged out successfully.");
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
>>>>>>> e65c010ef2fca80349078645d5d9912eb28ef042
        }

        public Task<ApiResponse> ResendConfirmationAccountAsync(string phone)
        {
            throw new NotImplementedException();
        }

<<<<<<< HEAD
        public Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            throw new NotImplementedException();
=======
        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto is null)
                return ApiResponseFactory.BadRequest("Reset data is required.");

            var validationResult = ValidationHelper.ModelValidation(dto);
            if (!validationResult.Success)
                return validationResult;

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
>>>>>>> e65c010ef2fca80349078645d5d9912eb28ef042
        }

        public Task<ApiResponse> UpdateUserProfileAsync(UpdateUserDTO dto)
        {
            throw new NotImplementedException();
        }

<<<<<<< HEAD
        public Task<ApiResponse> UploadUserPhotoAsync(Guid userId, UploadUserPhotoDTO dto)
        {
            throw new NotImplementedException();
=======
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
>>>>>>> e65c010ef2fca80349078645d5d9912eb28ef042
        }
    }
}
