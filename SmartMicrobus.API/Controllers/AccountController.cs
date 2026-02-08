using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Account;
using SmartMicrobus.Core.ServiceContracts.Account;
using System.Threading.Tasks;

namespace SmartMicrobus.API.Controllers
{
    [ApiVersion("1.0")]
    public class AccountController(IAuthService authService) : CustomControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok("Login endpoint");
        }

        [HttpPost]
        [Route("RegisterDriver")]
        public async Task<IActionResult> RegisterDriver([FromBody] RegisterDriverDTO registerDriverDTO)
        {
            if (registerDriverDTO is null)
            {
                throw new ArgumentNullException(nameof(registerDriverDTO), "RegisterDriverDTO cannot be null.");
            }
            
            var response = await authService.RegisterDriverAsync(registerDriverDTO);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("RegisterPassanger")]
        public async Task<IActionResult> RegisterPassenger([FromBody] RegisterPassengerDTO registerPassengerDTO)
        {
            if (registerPassengerDTO is null)
            {
                throw new ArgumentNullException(nameof(registerPassengerDTO), "RegisterPassengerDTO cannot be null.");
            }

            var response = await authService.RegisterPassengerAsync(registerPassengerDTO);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        }
}
