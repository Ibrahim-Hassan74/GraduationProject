using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Account;

namespace SmartMicrobus.API.Controllers
{
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            
            return StatusCode(response.StatusCode , response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var response = await _authService.ForgotPasswordAsync(dto);
            
            return StatusCode(response.StatusCode , response);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO dto)
        {
            var response = await _authService.VerifyOtpAsync(dto);
            
            return StatusCode(response.StatusCode , response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var response = await _authService.ResetPasswordAsync(dto);
            
            return StatusCode(response.StatusCode , response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Guid userId)
        {
            var response = await _authService.LogoutAsync(userId);
            
            return StatusCode(response.StatusCode , response);
        }
    }
}
