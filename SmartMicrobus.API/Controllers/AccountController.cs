using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Account;
using System.Security.Claims;

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

        [HttpPost("register-driver")]
        public async Task<IActionResult> RegisterDriver([FromBody] RegisterDriverDTO registerDriverDTO)
        {
            if (registerDriverDTO is null)
            {
                throw new ArgumentNullException(nameof(registerDriverDTO), "RegisterDriverDTO cannot be null.");
            }
            
            var response = await _authService.RegisterDriverAsync(registerDriverDTO);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("register-passanger")]
        public async Task<IActionResult> RegisterPassenger([FromBody] RegisterPassengerDTO registerPassengerDTO)
        {
            if (registerPassengerDTO is null)
            {
                throw new ArgumentNullException(nameof(registerPassengerDTO), "RegisterPassengerDTO cannot be null.");
            }

            var response = await _authService.RegisterPassengerAsync(registerPassengerDTO);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(id))
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid user."));

            var response = await _authService.LogoutAsync(Guid.Parse(id));
            
            return StatusCode(response.StatusCode , response);
        }

        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel model)
        {
            var response = await _authService.RefreshTokenAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("upload-photo")]
        [Authorize]
        public async Task<IActionResult> UploadUserPhoto([FromForm] UploadUserPhotoDTO dto)
        {
            var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
                return StatusCode(StatusCodes.Status401Unauthorized, ApiResponseFactory.Unauthorized());

            var result = await _authService.UploadUserPhotoAsync(Guid.Parse(userIdFromToken), dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-photo")]
        [Authorize]
        public async Task<IActionResult> DeleteUserPhoto()
        {
            var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
                return StatusCode(StatusCodes.Status401Unauthorized, ApiResponseFactory.Unauthorized());

            var result = await _authService.DeleteUserPhotoAsync(Guid.Parse(userIdFromToken));
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid user."));
            var result = await _authService.DeleteAccountAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountDTO dto)
        {
            var response = await _authService.ConfirmAccountAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ForgotPasswordDTO dto)
        {
            if (dto is null)
                return BadRequest(ApiResponseFactory.BadRequest("Phone number is required."));

            var response = await _authService.ResendConfirmationAccountAsync(dto.PhoneNumber);
            return StatusCode(response.StatusCode, response);
        }

    }
}
