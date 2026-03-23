namespace SmartMicrobus.Core.Domain.Entities
{
    public class DriverReportReason
    {
        public Guid DriverReportId { get; set; }
        public DriverReport DriverReport { get; set; } = null!;

        public int ReportReasonId { get; set; }
        public ReportReason ReportReason { get; set; } = null!;
    }
}
