using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Manager;
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
    }
}
