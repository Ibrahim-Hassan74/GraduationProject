using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class DriverReport : BaseEntity<Guid>
    {
        public Guid PassengerId { get; set; }
        public Passenger Passenger { get; set; } = null!;

        public Guid? DriverId { get; set; }
        public Driver? Driver { get; set; }

        public string PlateNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public ICollection<DriverReportReason> Reasons { get; set; } = new List<DriverReportReason>();
    }
}
