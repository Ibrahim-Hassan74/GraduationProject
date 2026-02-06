using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartMicrobus.API.Controllers
{
    public class AccountController : CustomControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok("Login endpoint");
        }
    }
}
