namespace SmartMicrobus.Core.Domain.Entities
{
    public class ReportReason
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        public ICollection<DriverReportReason> DriverReports { get; set; } = new List<DriverReportReason>();
    }
}
