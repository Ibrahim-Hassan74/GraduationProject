using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverHistoryRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int PageSize { get; set; } = 10;
        [Range(1, int.MaxValue, ErrorMessage = "Page count must be greater than 0.")]
        public int PageNumber { get; set; } = 1;
    }
}
