using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.Helper
{
    public static class ApiResponseFactory
    {
        public static ApiErrorResponse Failure(string message, int statusCode, params string[] errors)
        {
            return new ApiErrorResponse
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors.ToList()
            };
        }

        public static ApiResponse Success(string message)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }
        public static ApiResponseWithData<T> Success<T>(string message, T data)
        {
            return new ApiResponseWithData<T>(message, data);
        }
        /// <summary>
        /// Creates a response with the specified status code and optional errors.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="message">The message to return.</param>
        /// <param name="errors">Optional list of detailed errors.</param>
        /// <returns>An <see cref="ApiResponse"/> or <see cref="ApiErrorResponse"/>.</returns>
        private static ApiResponse Create(int statusCode, string message, List<string>? errors = null)
        {
            if (errors is not null)
                return new ApiErrorResponse
                {
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors,
                    Success = false
                };

            return new ApiResponse
            {
                StatusCode = statusCode,
                Message = message,
                Success = false
            };
        }

        /// <summary>
        /// Creates a 400 Bad Request response.
        /// </summary>
        public static ApiResponse BadRequest(string message = "Bad Request", List<string>? errors = null)
            => Create(400, message, errors);

        /// <summary>
        /// Creates a 401 Unauthorized response.
        /// </summary>
        public static ApiResponse Unauthorized(string message = "Unauthorized", List<string>? errors = null)
            => Create(401, message, errors);

        /// <summary>
        /// Creates a 403 Forbidden response.
        /// </summary>
        public static ApiResponse Forbidden(string message = "Forbidden", List<string>? errors = null)
            => Create(403, message, errors);

        /// <summary>
        /// Creates a 404 Not Found response.
        /// </summary>
        public static ApiResponse NotFound(string message = "Not Found", List<string>? errors = null)
            => Create(404, message, errors);

        /// <summary>
        /// Creates a 409 Conflict response.
        /// </summary>
        public static ApiResponse Conflict(string message = "Conflict", List<string>? errors = null)
            => Create(409, message, errors);

        /// <summary>
        /// Creates a 410 Gone response.
        /// </summary>
        public static ApiResponse Gone(string message = "Gone", List<string>? errors = null)
            => Create(410, message, errors);

        /// <summary>
        /// Creates a 415 Unsupported Media Type response.
        /// </summary>
        public static ApiResponse UnsupportedMediaType(string message = "Unsupported Media Type", List<string>? errors = null)
            => Create(415, message, errors);

        /// <summary>
        /// Creates a 429 Too Many Requests response.
        /// </summary>
        public static ApiResponse TooManyRequests(string message = "Too Many Requests", List<string>? errors = null)
            => Create(429, message, errors);

        /// <summary>
        /// Creates a 500 Internal Server Error response.
        /// </summary>
        public static ApiResponse InternalServerError(string message = "Internal Server Error", List<string>? errors = null)
            => Create(500, message, errors);

        /// <summary>
        /// Creates a 503 Service Unavailable response.
        /// </summary>
        public static ApiResponse ServiceUnavailable(string message = "Service Unavailable", List<string>? errors = null)
            => Create(503, message, errors);

        /// <summary>
        /// Creates a 501 Not Implemented response.
        /// </summary>
        public static ApiResponse NotImplemented(string message = "Not Implemented", List<string>? errors = null)
            => Create(501, message, errors);

        /// <summary>
        /// Creates a 408 Request Timeout response.
        /// </summary>
        public static ApiResponse RequestTimeout(string message = "Request Timeout", List<string>? errors = null)
            => Create(408, message, errors);

        /// <summary>
        /// Generates query parameters for authentication redirect.
        /// </summary>
        public static Dictionary<string, string?> BuildAuthQueryParams(ApiSuccessResponse res)
        {
            return new Dictionary<string, string?>
            {
                ["token"] = res.Token,
                ["refreshToken"] = res.RefreshToken,
                ["expiration"] = res.Expiration?.ToString("o"),
                ["refreshTokenExpirationDateTime"] = res.RefreshTokenExpirationDateTime.ToString("o"),
                ["userName"] = res.UserName,
                ["phone"] = res.Phone
            };
        }
    }
}
