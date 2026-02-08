using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Account;

namespace SmartMicrobus.Core.Services.Account
{
    public class AuthService(UserManager<ApplicationUser> userManager, IDriverRepository driverRepository,
        IPassangerRepository passangerRepository) : IAuthService
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
