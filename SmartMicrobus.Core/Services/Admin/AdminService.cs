using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Manager;
using SmartMicrobus.Core.DTO.Admin;
using SmartMicrobus.Core.DTO.Account;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using managerEntity = SmartMicrobus.Core.Domain.Entities.Manager;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Admin;

namespace SmartMicrobus.Core.Services.Admin
{
    public class AdminService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IAdminService
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

            var result = await userManager.CreateAsync(user, registerManagerDTO.Password);
            if (!result.Succeeded)
            {
                return ApiResponseFactory.Failure("Failed to create manager user", 400, result.Errors.Select(e => e.Description).ToArray());
            }

            await EnsureRoleExistsAndAssignAsync(user, UserRole.Manager.ToString());

            var manager = new managerEntity
            {
                Id = user.Id,
                StationId = registerManagerDTO.StationId
            };

            await unitOfWork.ManagerRepository.AddAsync(manager);
            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Manager added successfully");
        }

        private async Task EnsureRoleExistsAndAssignAsync(ApplicationUser user, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });

            await userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<ApiResponse> GetUsersAsync(GetUsersQuery query)
        {
            // Use repository to fetch projected users with their role and total count (pre-paged)
            var (items, totalCount) = await unitOfWork.UserRepository.GetPagedUsersWithRolesAsync(query);

            // Map projection to ApplicationUserResponse
            var responses = items.Select(u => new ApplicationUserResponse
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                PhoneNumber = u.PhoneNumber,
                PhotoUrl = u.PhotoName,
                Roles = u.Role,
                IsActive = !u.IsDeleted && (!u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow),
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

            return ApiResponseFactory.Success("Users retrieved successfully", paged);
        }

        public async Task<ApplicationUserResponse?> GetUserByIdAsync(Guid userId)
        {
            var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            var response = new ApplicationUserResponse
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.PhotoName,
                Roles = user.Role,
                IsActive = !user.IsDeleted && (!user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.UtcNow),
                IsConfirmed = user.IsConfirmed
            };

            return response;
        }

        public async Task<ApiResponse> LockAccountAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null) return ApiResponseFactory.NotFound("User not found.");

            var roles = await userManager.GetRolesAsync(user);
            if (roles.Any(r => string.Equals(r, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                return ApiResponseFactory.Forbidden("Cannot lock an admin account.");
            }

            await userManager.SetLockoutEnabledAsync(user, true);
            var lockoutEnd = DateTimeOffset.UtcNow.AddYears(1000);
            var result = await userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            if (!result.Succeeded)
                return ApiResponseFactory.Failure("Failed to lock account.", 400, result.Errors.Select(e => e.Description).ToArray());

            return ApiResponseFactory.Success("Account locked successfully.");
        }

        public async Task<ApiResponse> UnlockAccountAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null) return ApiResponseFactory.NotFound("User not found.");

            var result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            if (!result.Succeeded)
                return ApiResponseFactory.Failure("Failed to unlock account.", 400, result.Errors.Select(e => e.Description).ToArray());

            return ApiResponseFactory.Success("Account unlocked successfully.");
        }


    }
}
