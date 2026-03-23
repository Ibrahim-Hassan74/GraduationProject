namespace SmartMicrobus.Core.DTO.Report
{
    public class CreateReportRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public List<int> ReasonIds { get; set; } = new List<int>();
        public string? Description { get; set; }
    }
}
