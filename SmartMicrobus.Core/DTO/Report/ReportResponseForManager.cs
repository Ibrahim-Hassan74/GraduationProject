namespace SmartMicrobus.Core.DTO.Report
{
    public class ReportResponseForManager : ReportResponseWithDetails
    {
        public string PassengerName { get; set; } = string.Empty;
        public string? DriverName { get; set; }
        public Guid PassangerId { get; set; }
        public Guid? DriverId { get; set; }


    }
}
