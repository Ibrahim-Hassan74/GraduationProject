using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Security.Claims;
using System.Security.Cryptography;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.Services.Account
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly IDriverRepository _driverRepository;
        private readonly IPassengerRepository _passangerRepository;
        private readonly IOtpService _otpService;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IWhatsAppService whatsAppService,
            IUnitOfWork unitOfWork,
            IImageService imageService,
            IOtpService otpService,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _whatsAppService = whatsAppService;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _driverRepository = _unitOfWork.DriverRepository;
            _passangerRepository = _unitOfWork.PassengerRepository;
            _otpService = otpService;
            _roleManager = roleManager;
        }

        // Parse stored token format:
        // otp|expiryUnix[|sentUnix[|resendCount]]

        public async Task<ApiResponse> RegisterDriverAsync(RegisterDriverDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.PhoneNumber,
                PhoneNumber = dto.PhoneNumber,
                DisplayName = dto.DisplayName,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return ApiResponseFactory.Failure("Failed to register driver.", 400, [.. result.Errors.Select(e => e.Description)]);
            }

            await EnsureRoleExistsAndAssignAsync(user, UserRole.Driver.ToString());

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);
                var ok = await _whatsAppService.SendOTPAsync(user.PhoneNumber!, otp);

                if (!ok)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception("WhatsApp service returned false.");
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];
                    return ApiResponseFactory.TooManyRequests($"Please wait {seconds} seconds.");
                }
                return ApiResponseFactory.InternalServerError("Failed to send OTP. Please try again later.");
            }

            var driver = new Driver
            {
                Id = user.Id,
                LicenseNumber = dto.LicenseNumber
            };

            await _driverRepository.AddAsync(driver);
            await _unitOfWork.CompleteAsync();
            return ApiResponseFactory.Success("Driver registered successfully.");
        }

        public async Task<ApiResponse> RegisterPassengerAsync(RegisterPassengerDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.PhoneNumber,
                PhoneNumber = dto.PhoneNumber,
                DisplayName = dto.Name,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return ApiResponseFactory.Failure("Failed to register passenger.", 400, [.. result.Errors.Select(e => e.Description)]);
            }

            await EnsureRoleExistsAndAssignAsync(user, UserRole.Passenger.ToString());

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);
                var ok = await _whatsAppService.SendOTPAsync(user.PhoneNumber!, otp);
                if (!ok)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception("WhatsApp service returned false.");
                }
            }
            catch (Exception ex)
            {
                //log the error
                //Console.WriteLine($"{ex.Message},\n {ex.InnerException}");
                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];
                    return ApiResponseFactory.TooManyRequests($"Please wait {seconds} seconds.");
                }
                return ApiResponseFactory.InternalServerError("Failed to send OTP. Please try again later.");
            }

            var passenger = new Passenger
            {
                Id = user.Id,
            };
            var response = await _passangerRepository.AddAsync(passenger);

            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Passenger registered successfully.");
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

            if (!user.PhoneNumberConfirmed)
            {
                return ApiResponseFactory.Unauthorized("Phone number not confirmed. Please confirm your account before logging in.");
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

            if (!user.PhoneNumberConfirmed)
            {
                return ApiResponseFactory.Unauthorized("Phone number not confirmed. Please confirm your account before resetting password.");
            }

            var verifyResult = await _otpService.VerifyOtpAsync(user, dto.Otp);

            if (!verifyResult)
                return ApiResponseFactory.BadRequest("No OTP found or it has expired.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return ApiResponseFactory.Success("OTP verified successfully.", new VerifyOtpResponse { Token = resetToken, UserId = user.Id.ToString() });
        }

        public async Task<ApiResponse> ConfirmAccountAsync(ConfirmAccountDTO dto)
        {
            if (dto is null)
                return ApiResponseFactory.BadRequest("Confirmation data is required.");

            var validationResult = ValidationHelper.ModelValidation(dto);
            if (!validationResult.Success)
                return validationResult;

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (user is null)
                return ApiResponseFactory.BadRequest("Invalid phone number or OTP.");

            if (user.PhoneNumberConfirmed)
                return ApiResponseFactory.BadRequest("Account already confirmed.");

            var ok = await _otpService.VerifyOtpAsync(user, dto.Otp);

            if (!ok)
                return ApiResponseFactory.BadRequest("Invalid or expired OTP.");

            user.PhoneNumberConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return ApiResponseFactory.Failure("Failed to confirm account.", 400, updateResult.Errors.Select(e => e.Description).ToArray());
            }

            return ApiResponseFactory.Success("Account confirmed successfully.");
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

            if (!user.PhoneNumberConfirmed)
            {
                return ApiResponseFactory.Unauthorized("Phone number not confirmed. Please confirm your account before resetting password.");
            }

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);
                var ok = await _whatsAppService.SendOTPAsync(user.PhoneNumber!, otp);

                if (!ok)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception("WhatsApp service returned false.");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];
                    return ApiResponseFactory.TooManyRequests($"Please wait {seconds} seconds.");
                }

                return ApiResponseFactory.InternalServerError("Failed to send OTP. Please try again later.");
            }

            return ApiResponseFactory.Success("If an account with the provided phone number exists, an OTP has been sent.");
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

        public async Task<ApiResponse> ResendConfirmationAccountAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return ApiResponseFactory.BadRequest("Phone number is required.");

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phone);

            if (user is null)
            {
                return ApiResponseFactory.Success("If an account with the provided phone number exists, an OTP has been sent.");
            }

            if (user.PhoneNumberConfirmed)
                return ApiResponseFactory.BadRequest("Account already confirmed.");


            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);
                var result = await _whatsAppService.SendOTPAsync(user.PhoneNumber!, otp);
                if (!result)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];
                    return ApiResponseFactory.TooManyRequests($"Please wait {seconds} seconds.");
                }
                return ApiResponseFactory.InternalServerError("Failed to send OTP. Please try again later.");
            }

            return ApiResponseFactory.Success("If an account with the provided phone number exists, an OTP has been sent.");
        }

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

            if(!user.PhoneNumberConfirmed)
                return ApiResponseFactory.Unauthorized("Phone number not confirmed. Please confirm your account before resetting password.");

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

        public async Task<ApiResponse> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ApiResponseFactory.NotFound("User not found");

            var deleted = await _userManager.DeleteAsync(user);

            if (!deleted.Succeeded)
                return ApiResponseFactory.Failure("Failed to delete account", 400, deleted.Errors.Select(e => e.Description).ToArray());

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

        private async Task EnsureRoleExistsAndAssignAsync(ApplicationUser user, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });

            await _userManager.AddToRoleAsync(user, roleName);
        }
    }
}
