using SmartMicrobus.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Report
{
    public class GetReportsQuery
    {
        [StringLength(200)]
        public string? PlateNumber { get; set; } 

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
        public ReportStatus? Status { get; set; }
        public SortOrderOptions OrderOptions { get; set; } = SortOrderOptions.ASC;
        public OrderByOptions OrderByOptions { get; set; } = OrderByOptions.CreatedAt;

    }
}
