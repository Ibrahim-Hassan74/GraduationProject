using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Security.Claims;

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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<AuthService> _localizer;
        private readonly ICustomWhatsAppService _customWhatsAppService;
        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IWhatsAppService whatsAppService,
            IUnitOfWork unitOfWork,
            IImageService imageService,
            IOtpService otpService,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            IConfiguration configuration,
            IStringLocalizer<AuthService> localizer,
            ICustomWhatsAppService customWhatsAppService)
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
            _mapper = mapper;
            _configuration = configuration;
            _localizer = localizer;
            _customWhatsAppService = customWhatsAppService;
        }

        // Parse stored token format:
        // otp|expiryUnix[|sentUnix[|resendCount]]

        public async Task<ApiResponse> RegisterDriverAsync(RegisterDriverDTO
            dto)
        {
            if (dto is null)
                return ApiResponseFactory.BadRequest("Driver registration data is required.");

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (user == null)
            {
                return ApiResponseFactory.Failure(
                    _localizer["RegisterDriver_Failed"],
                    400
                );
            }

            var PasswordResult = await _userManager.AddPasswordAsync(user, dto.Password);
            if (!PasswordResult.Succeeded)
                return ApiResponseFactory.BadRequest(PasswordResult.Errors.First().Description);

            await EnsureRoleExistsAndAssignAsync(user, UserRole.Driver.ToString());

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);
                //var ok = await _whatsAppService.SendOTPAsync(
                //    user.PhoneNumber!,
                //    otp,
                //    _configuration["WhatsAppTemplates:ConfirmAccount"]
                //);
                
                var ok = await _customWhatsAppService.SendMessageAsync(
                    user.PhoneNumber!, _localizer["ConfirmAccount_Message", otp]
                );

                if (!ok)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception(_localizer["WhatsApp_Send_Failed"]);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];

                    return ApiResponseFactory.TooManyRequests(
                        _localizer["OTP_Cooldown", seconds]
                    );
                }

                return ApiResponseFactory.InternalServerError(
                    _localizer["OTP_Send_Failed"]
                );
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
                return ApiResponseFactory.Failure(
                    _localizer["RegisterPassenger_Failed"],
                    400,
                    [.. result.Errors.Select(e => e.Description)]
                );
            }

            await EnsureRoleExistsAndAssignAsync(user, UserRole.Passenger.ToString());

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);
                //var ok = await _whatsAppService.SendOTPAsync(
                //    user.PhoneNumber!,
                //    otp,
                //    _configuration["WhatsAppTemplates:ConfirmAccount"]
                //);

                var ok = await _customWhatsAppService.SendMessageAsync(
                    user.PhoneNumber!, _localizer["ConfirmAccount_Message", otp]
                );

                if (!ok)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception(_localizer["WhatsApp_Send_Failed"]);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];

                    return ApiResponseFactory.TooManyRequests(
                        _localizer["OTP_Cooldown", seconds]
                    );
                }

                return ApiResponseFactory.InternalServerError(
                    _localizer["OTP_Send_Failed"]
                );
            }

            var passenger = new Passenger
            {
                Id = user.Id,
            };

            await _passangerRepository.AddAsync(passenger);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(
                _localizer["RegisterPassenger_Success"]
            );
        }

        public async Task<ApiResponse> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO is null)
                return ApiResponseFactory.BadRequest(
                    _localizer["Login_Data_Required"]
                );

            var validationResult = ValidationHelper.ModelValidation(loginDTO);
            if (!validationResult.Success)
                return validationResult;

            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == loginDTO.PhoneNumber);

            if (user is null)
            {
                return ApiResponseFactory.Unauthorized(
                    _localizer["Login_Invalid_Credentials"]
                );
            }

            if (!user.PhoneNumberConfirmed)
            {
                return ApiResponseFactory.Unauthorized(
                    _localizer["Login_Phone_Not_Confirmed"]
                );
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                loginDTO.Password,
                lockoutOnFailure: true
            );

            if (signInResult.IsLockedOut)
            {
                return ApiResponseFactory.Forbidden(
                    _localizer["Login_User_Locked"]
                );
            }

            if (signInResult.IsNotAllowed)
            {
                return ApiResponseFactory.Forbidden(
                    _localizer["Login_Not_Allowed"]
                );
            }

            if (!signInResult.Succeeded)
            {
                return ApiResponseFactory.Unauthorized(
                    _localizer["Login_Invalid_Credentials"]
                );
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
                return ApiResponseFactory.BadRequest(
                    _localizer["VerifyOtp_Data_Required"]
                );

            var validationResult = ValidationHelper.ModelValidation(dto);
            if (!validationResult.Success)
                return validationResult;

            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (user is null)
                return ApiResponseFactory.BadRequest(
                    _localizer["VerifyOtp_Invalid_Data"]
                );

            if (!user.PhoneNumberConfirmed)
            {
                return ApiResponseFactory.Unauthorized(
                    _localizer["VerifyOtp_Phone_Not_Confirmed_Reset"]
                );
            }

            var verifyResult = await _otpService.VerifyOtpAsync(user, dto.Otp);

            if (!verifyResult)
                return ApiResponseFactory.BadRequest(
                    _localizer["VerifyOtp_Not_Found_Or_Expired"]
                );

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return ApiResponseFactory.Success(
                _localizer["VerifyOtp_Success"],
                new VerifyOtpResponse
                {
                    Token = resetToken,
                    UserId = user.Id.ToString()
                }
            );
        }

        public async Task<ApiResponse> ConfirmAccountAsync(ConfirmAccountDTO dto)
        {
            if (dto is null)
                return ApiResponseFactory.BadRequest(
                    _localizer["ConfirmAccount_Data_Required"]
                );

            var validationResult = ValidationHelper.ModelValidation(dto);
            if (!validationResult.Success)
                return validationResult;

            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (user is null)
                return ApiResponseFactory.BadRequest(
                    _localizer["VerifyOtp_Invalid_Data"]
                );

            if (user.PhoneNumberConfirmed)
                return ApiResponseFactory.BadRequest(
                    _localizer["ConfirmAccount_Already_Confirmed"]
                );

            var ok = await _otpService.VerifyOtpAsync(user, dto.Otp);

            if (!ok)
                return ApiResponseFactory.BadRequest(
                    _localizer["ConfirmAccount_Invalid_Or_Expired_Otp"]
                );

            user.PhoneNumberConfirmed = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return ApiResponseFactory.Failure(
                    _localizer["ConfirmAccount_Failed"],
                    400,
                    updateResult.Errors.Select(e => e.Description).ToArray()
                );
            }

            return ApiResponseFactory.Success(
                _localizer["ConfirmAccount_Success"]
            );
        }

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return ApiResponseFactory.BadRequest(
                    _localizer["ForgotPassword_Phone_Required"]
                );

            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (user is null)
            {
                return ApiResponseFactory.NotFound(
                    _localizer["ForgotPassword_User_Not_Found"]
                );
            }

            if (!user.PhoneNumberConfirmed)
            {
                return ApiResponseFactory.Unauthorized(
                    _localizer["VerifyOtp_Phone_Not_Confirmed_Reset"]
                );
            }

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);

                //var ok = await _whatsAppService.SendOTPAsync(
                //    user.PhoneNumber!,
                //    otp,
                //    _configuration["WhatsAppTemplates:ResetPassword"]
                //);

                var ok = await _customWhatsAppService.SendMessageAsync(
                    user.PhoneNumber!, _localizer["ResetPassword_Message", otp]
                );

                if (!ok)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception(_localizer["WhatsApp_Send_Failed"]);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];

                    return ApiResponseFactory.TooManyRequests(
                        _localizer["OTP_Cooldown", seconds]
                    );
                }

                return ApiResponseFactory.InternalServerError(
                    _localizer["OTP_Send_Failed"]
                );
            }

            return ApiResponseFactory.Success(
                _localizer["ForgotPassword_Success_Generic"]
            );
        }

        public async Task<ApiResponse> LogoutAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return ApiResponseFactory.BadRequest(
                    _localizer["Logout_UserId_Required"]
                );

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpirationDateTime = DateTimeOffset.MinValue;

                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignOutAsync();

            return ApiResponseFactory.Success(
                _localizer["Logout_Success"]
            );
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
            catch (SecurityTokenException)
            {
                return ApiResponseFactory.Failure(
                    _localizer["RefreshToken_Invalid_Token"],
                    400,
                    _localizer["RefreshToken_Access_Invalid"]
                );
            }

            if (principal is null)
                return ApiResponseFactory.Failure(
                    _localizer["RefreshToken_Invalid_Token"],
                    400,
                    _localizer["RefreshToken_Access_Invalid"]
                );

            var phone = principal.FindFirstValue(ClaimTypes.MobilePhone);

            if (string.IsNullOrWhiteSpace(phone))
                return ApiResponseFactory.Failure(
                    _localizer["RefreshToken_Invalid_Token"],
                    400,
                    _localizer["RefreshToken_Claim_Missing"]
                );

            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.PhoneNumber == phone);

            if (user is null)
                return ApiResponseFactory.Failure(
                    _localizer["RefreshToken_User_Not_Found"],
                    404,
                    _localizer["RefreshToken_User_Not_Exist"]
                );

            if (user.RefreshToken != model.RefreshToken ||
                user.RefreshTokenExpirationDateTime <= DateTimeOffset.UtcNow)
            {
                return ApiResponseFactory.Failure(
                    _localizer["RefreshToken_Invalid_Refresh"],
                    400,
                    _localizer["RefreshToken_Invalid_Or_Expired"]
                );
            }

            bool rememberMe =
                bool.TryParse(principal.FindFirst("remember_me")?.Value, out var rm) && rm;

            var authResponse = await _jwtService.CreateJwtToken(user, rememberMe) as ApiSuccessResponse;

            // Rotate refresh token
            user.RefreshToken = authResponse?.RefreshToken;
            user.RefreshTokenExpirationDateTime = authResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(user);

            authResponse.Success = true;
            authResponse.StatusCode = 200;
            authResponse.Message = _localizer["RefreshToken_Success"];

            return authResponse;
        }

        public async Task<ApiResponse> ResendConfirmationAccountAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return ApiResponseFactory.BadRequest(
                    _localizer["ForgotPassword_Phone_Required"]
                );

            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == phone);

            if (user is null || user.PhoneNumberConfirmed)
            {
                return ApiResponseFactory.Success(
                    _localizer["ResendConfirmation_Success_Generic"]
                );
            }

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user);

                //var result = await _whatsAppService.SendOTPAsync(
                //    user.PhoneNumber!,
                //    otp,
                //    _configuration["WhatsAppTemplates:ConfirmAccount"]
                //);

                var result = await _customWhatsAppService.SendMessageAsync(
                    user.PhoneNumber!, _localizer["ConfirmAccount_Message", otp]
                );

                if (!result)
                {
                    await _otpService.ClearOtpAsync(user);
                    throw new Exception(_localizer["WhatsApp_Send_Failed"]);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("OTP_COOLDOWN"))
                {
                    var seconds = ex.Message.Split(':')[1];

                    return ApiResponseFactory.TooManyRequests(
                        _localizer["OTP_Cooldown", seconds]
                    );
                }

                return ApiResponseFactory.InternalServerError(
                    _localizer["OTP_Send_Failed"] 
                );
            }

            return ApiResponseFactory.Success(
                _localizer["ResendConfirmation_Success_Generic"]
            );
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto is null)
                return ApiResponseFactory.BadRequest(
                    _localizer["ResetPassword_Data_Required"]
                );

            var validationResult = ValidationHelper.ModelValidation(dto);
            if (!validationResult.Success)
                return validationResult;

            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

            if (user is null)
                return ApiResponseFactory.NotFound(
                    _localizer["RefreshToken_User_Not_Found"]
                );

            if (!user.PhoneNumberConfirmed)
                return ApiResponseFactory.Unauthorized(
                    _localizer["VerifyOtp_Phone_Not_Confirmed_Reset"]
                );

            var result = await _userManager.ResetPasswordAsync(
                user,
                dto.Token,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                return ApiResponseFactory.BadRequest(
                    _localizer["ResetPassword_Failed"],
                    errors
                );
            }

            return ApiResponseFactory.Success(
                _localizer["ResetPassword_Success"]
            );
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
                return ApiResponseFactory.NotFound(
                    _localizer["RefreshToken_User_Not_Found"]
                );

            var file = dto.File;

            if (file == null || file.Length == 0)
                return ApiResponseFactory.BadRequest(
                    _localizer["UploadPhoto_No_File"]
                );

            if (!file.ContentType.StartsWith("image/"))
                return ApiResponseFactory.BadRequest(
                    _localizer["UploadPhoto_Invalid_Type"]
                );

            if (file.Length > 5 * 1024 * 1024)
                return ApiResponseFactory.BadRequest(
                    _localizer["UploadPhoto_Size_Exceeded"]
                );

            if (user.Photo != null)
            {
                _imageService.DeleteImageAsync(user.Photo.ImageName);
                await _unitOfWork.PhotoRepository.DeleteAsync(user.Photo.Id);
                user.Photo = null;
            }

            var folderName = user.PhoneNumber?
                .Replace(" ", "")
                .ToLowerInvariant();

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

            return ApiResponseFactory.Success(
                _localizer["UploadPhoto_Success"]
            );
        }

        public async Task<ApiResponse> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ApiResponseFactory.NotFound(
                    _localizer["RefreshToken_User_Not_Found"]
                );

            var deleted = await _userManager.DeleteAsync(user);

            if (!deleted.Succeeded)
                return ApiResponseFactory.Failure(
                    _localizer["DeleteAccount_Failed"],
                    400,
                    deleted.Errors.Select(e => e.Description).ToArray()
                );

            return ApiResponseFactory.Success(
                _localizer["DeleteAccount_Success"]
            );
        }

        public async Task<ApiResponse> DeleteUserPhotoAsync(Guid userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Photo)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ApiResponseFactory.NotFound(
                    _localizer["RefreshToken_User_Not_Found"]
                );

            if (user.Photo == null)
                return ApiResponseFactory.BadRequest(
                    _localizer["DeleteUserPhoto_Not_Found"]
                );

            _imageService.DeleteImageAsync(user.Photo.ImageName);
            await _unitOfWork.PhotoRepository.DeleteAsync(user.Photo.Id);
            user.Photo = null;

            await _unitOfWork.CompleteAsync();
            await _userManager.UpdateAsync(user);
            return ApiResponseFactory.Success(
                _localizer["DeleteUserPhoto_Success"]
            );
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

        public async Task<ApplicationUserResponse> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.Users.Include(u => u.Photo).FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));
            if (user == null) return null;

            var response = _mapper.Map<ApplicationUserResponse>(user);
            response.Roles = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            return response;
        }
    }
}
