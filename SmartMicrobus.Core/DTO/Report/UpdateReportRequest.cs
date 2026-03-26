using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Report
{
    public class UpdateReportRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        public List<int> ReasonIds { get; set; } = new List<int>();

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
