using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.DTO.Admin;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Manager;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.Resources;
using SmartMicrobus.Core.ServiceContracts.Admin;
using System;
using System.Linq;
using managerEntity = SmartMicrobus.Core.Domain.Entities.Manager;

namespace SmartMicrobus.Core.Services.Admin
{
    public class AdminService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IStringLocalizer<AdminService> localizer) : IAdminService
    {
        public async Task<ApiResponse> AddManagerAsync(RegisterManagerDTO registerManagerDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registerManagerDTO.PhoneNumber,
                PhoneNumber = registerManagerDTO.PhoneNumber,
                DisplayName = registerManagerDTO.DisplayName,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(
                user,
                registerManagerDTO.Password);

            if (!result.Succeeded)
            {
                return ApiResponseFactory.Failure(
                    localizer["CreateManager_Failed"],
                    400,
                    result.Errors.Select(e => e.Description).ToArray());
            }

            await EnsureRoleExistsAndAssignAsync(
                user,
                UserRole.Manager.ToString());

            var manager = new managerEntity
            {
                Id = user.Id,
                StationId = registerManagerDTO.StationId
            };

            await unitOfWork.ManagerRepository.AddAsync(manager);
            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(
                localizer["Manager_Added_Success"]);
        }

        private async Task EnsureRoleExistsAndAssignAsync(
            ApplicationUser user,
            string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(
                    new ApplicationRole { Name = roleName });
            }

            await userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<ApiResponse> GetUsersAsync(GetUsersQuery query)
        {
            var (items, totalCount) =
                await unitOfWork.UserRepository
                    .GetPagedUsersWithRolesAsync(query);

            var responses = items.Select(u => new ApplicationUserResponse
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                PhoneNumber = u.PhoneNumber,
                PhotoUrl = u.PhotoName,
                Roles = u.Role,
                IsActive = !u.IsDeleted &&
                           (!u.LockoutEnd.HasValue ||
                            u.LockoutEnd <= DateTimeOffset.UtcNow),
                IsConfirmed = u.IsConfirmed
            }).ToList();

            var page = Math.Max(1, query.PageNumber);
            var size = Math.Max(5, query.PageSize);

            var paged = new PagedResponse<ApplicationUserResponse>
            {
                Items = responses,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };

            return ApiResponseFactory.Success(
                localizer["Users_Retrieved_Success"],
                paged);
        }

        public async Task<ApplicationUserResponse?> GetUserByIdAsync(Guid userId)
        {
            var user = await unitOfWork.UserRepository
                .GetUserByIdAsync(userId);

            if (user == null)
                return null;

            return new ApplicationUserResponse
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.PhotoName,
                Roles = user.Role,
                IsActive = !user.IsDeleted &&
                           (!user.LockoutEnd.HasValue ||
                            user.LockoutEnd <= DateTimeOffset.UtcNow),
                IsConfirmed = user.IsConfirmed
            };
        }

        public async Task<ApiResponse> LockAccountAsync(Guid userId)
        {
            var user = await userManager
                .FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return ApiResponseFactory.NotFound(
                    localizer["User_Not_Found"]);
            }

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Any(r => string.Equals(
                r,
                UserRole.Admin.ToString(),
                StringComparison.OrdinalIgnoreCase)))
            {
                return ApiResponseFactory.Forbidden(
                    localizer["Cannot_Lock_Admin"]);
            }

            await userManager.SetLockoutEnabledAsync(user, true);

            var result = await userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow.AddYears(1000));

            if (!result.Succeeded)
            {
                return ApiResponseFactory.Failure(
                    localizer["LockAccount_Failed"],
                    400,
                    result.Errors.Select(e => e.Description).ToArray());
            }

            return ApiResponseFactory.Success(
                localizer["LockAccount_Success"]);
        }

        public async Task<ApiResponse> UnlockAccountAsync(Guid userId)
        {
            var user = await userManager
                .FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return ApiResponseFactory.NotFound(
                    localizer["User_Not_Found"]);
            }

            var result = await userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow);

            if (!result.Succeeded)
            {
                return ApiResponseFactory.Failure(
                    localizer["UnlockAccount_Failed"],
                    400,
                    result.Errors.Select(e => e.Description).ToArray());
            }

            return ApiResponseFactory.Success(
                localizer["UnlockAccount_Success"]);
        }

        public async Task<ApiResponse> DeleteManagerAsync(Guid managerId)
        {
            var manager = await unitOfWork.UserRepository
                .GetByIdWithUserAsync(managerId);

            if (manager == null)
            {
                return ApiResponseFactory.NotFound(
                    localizer["Manager_Not_Found"]);
            }

            var user = manager.ApplicationUser;
            var originalPhone = user.PhoneNumber;

            user.IsDeleted = true;
            user.PhoneNumber = $"DELETED_{user.Id}_{originalPhone}";
            user.UserName = user.PhoneNumber;
            user.PhoneNumberConfirmed = false;
            user.EmailConfirmed = false;
            user.RefreshToken = null;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return ApiResponseFactory.Failure(
                    localizer["DeleteManager_Failed"],
                    400,
                    result.Errors.Select(e => e.Description).ToArray());
            }

            return ApiResponseFactory.Success(
                localizer["DeleteManager_Success"]);
        }

        public async Task<ApiResponse> UpdateManagerStationAsync(Guid managerId, UpdateManagerStationDTO dto)
        {
            var manager = await unitOfWork.UserRepository
                .GetByIdWithUserAsync(managerId);

            if (manager == null)
            {
                return ApiResponseFactory.NotFound(
                    localizer["Manager_Not_Found"]);
            }

            var stationExists = await unitOfWork.StationRepository.GetByIdAsync(dto.StationId);

            if (stationExists == null)
            {
                return ApiResponseFactory.NotFound(
                    localizer["Station_Not_Found"]);
            }

            manager.StationId = dto.StationId;
            manager.ApplicationUser.DisplayName = dto.Name;
            await unitOfWork.ManagerRepository.UpdateAsync(manager);
            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(
                localizer["Manager_Station_Updated_Success"]);
        }
    }
}