using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Common;
using System.Security.Claims;

namespace SmartMicrobus.Core.ServiceContracts.Common
{
    /// <summary>
    /// JWT Service Interface
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT token using the given user's information and the configuration settings.
        /// </summary>
        /// <param name="user">ApplicationUser object</param>
        /// <param name="rememberMe">Indicates if the user wants to be remembered</param>
        /// <returns>AuthenticationResponse that includes token</returns>
        Task<ApiResponse> CreateJwtToken(ApplicationUser user, bool rememberMe);
        /// <summary>
        /// Get ClaimsPrincipal from JWT token
        /// </summary>
        /// <param name="token">Access Token</param>
        /// <returns></returns>
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
