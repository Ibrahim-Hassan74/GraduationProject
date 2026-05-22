using System.ComponentModel.DataAnnotations;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Report
{
    public class ReportResponse
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }

        public string Status { get; set; }
    }
}
