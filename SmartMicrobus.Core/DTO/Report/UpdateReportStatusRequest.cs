using System.ComponentModel.DataAnnotations;
using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Report
{
    public class UpdateReportStatusRequest
    {
        [Required]
        public ReportStatus Status { get; set; }
    }
}
