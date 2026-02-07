using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartMicrobus.API.Controllers
{
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok("Login endpoint");
        }
    }
}
