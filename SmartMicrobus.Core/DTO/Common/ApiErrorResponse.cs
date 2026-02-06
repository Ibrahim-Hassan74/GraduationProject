namespace SmartMicrobus.Core.DTO.Common
{
    /// <summary>
    /// Represents a standardized error response for the API.
    /// </summary>
    public class ApiErrorResponse : ApiResponse
    {
        /// <summary>
        /// A list of detailed error messages.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        public ApiErrorResponse()
        {
            Success = false;
        }
    }
}
