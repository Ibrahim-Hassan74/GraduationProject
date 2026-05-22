using SmartMicrobus.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Report
{
    public class ReportResponseWithDetails:ReportResponse
    {
      
        public List<string> Reasons { get; set; } = new List<string>();

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
