namespace SmartMicrobus.Core.DTO.Common
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }
}
