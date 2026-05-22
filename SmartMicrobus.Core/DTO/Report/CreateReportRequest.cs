using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Report
{
    public class CreateReportRequest
    {
        [Required(ErrorMessageResourceName = "RequiredPlateNumber",
                  ErrorMessageResourceType = typeof(Resources.DTO.Report.ReportValidationMessages))]
        [StringLength(10,
            MinimumLength = 4,
            ErrorMessageResourceName = "StringLengthPlateNumber",
            ErrorMessageResourceType = typeof(Resources.DTO.Report.ReportValidationMessages))]
        public string PlateNumber { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "RequiredReasonIds",
                  ErrorMessageResourceType = typeof(Resources.DTO.Report.ReportValidationMessages))]
        [MinLength(1,
            ErrorMessageResourceName = "MinLengthReasonIds",
            ErrorMessageResourceType = typeof(Resources.DTO.Report.ReportValidationMessages))]
        public List<int> ReasonIds { get; set; } = new List<int>();

        [StringLength(2000,
            ErrorMessageResourceName = "MaxLengthDescription",
            ErrorMessageResourceType = typeof(Resources.DTO.Report.ReportValidationMessages))]
        public string? Description { get; set; }
    }
}