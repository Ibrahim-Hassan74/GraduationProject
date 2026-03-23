using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Report
{
    public class ReportResponse
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public List<string> Reasons { get; set; } = new List<string>();

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
