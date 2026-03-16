using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.API.Controllers
{
    /// <summary>
    /// Base controller for the Smart Microbus API, providing common functionality for all controllers.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomControllerBase : ControllerBase
    {
        /// <summary>
        /// Constructor for CustomControllerBase, initializing the unit of work.
        /// </summary>

        public CustomControllerBase()
        {
            // This constructor can be used to initialize common services or properties
            // that all derived controllers might need.
        }
        /// <summary>
        /// Converts an ApiResponse object to an appropriate IActionResult based on its success status and HTTP status
        /// code.
        /// </summary>
        /// <remarks>This method maps common HTTP status codes (400, 404, 401) to their respective
        /// IActionResult types. For other status codes, a generic 500 Internal Server Error is returned.</remarks>
        /// <param name="response">The ApiResponse to convert. Determines the result type and status code of the returned IActionResult.</param>
        /// <returns>An IActionResult representing the outcome described by the ApiResponse. Returns Ok if successful; otherwise,
        /// returns a result corresponding to the ApiResponse's status code.</returns>
        protected IActionResult ToActionResult(ApiResponse response)
        {
            if (response.Success)
                return Ok(response);

            return response.StatusCode switch
            {
                400 => BadRequest(response),
                401 => Unauthorized(response),
                403 => StatusCode(StatusCodes.Status403Forbidden, response),
                404 => NotFound(response),
                408 => StatusCode(StatusCodes.Status408RequestTimeout, response),
                409 => Conflict(response),
                410 => StatusCode(StatusCodes.Status410Gone, response),
                415 => StatusCode(StatusCodes.Status415UnsupportedMediaType, response),
                429 => StatusCode(StatusCodes.Status429TooManyRequests, response),
                500 => StatusCode(StatusCodes.Status500InternalServerError, response),
                501 => StatusCode(StatusCodes.Status501NotImplemented, response),
                503 => StatusCode(StatusCodes.Status503ServiceUnavailable, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}
