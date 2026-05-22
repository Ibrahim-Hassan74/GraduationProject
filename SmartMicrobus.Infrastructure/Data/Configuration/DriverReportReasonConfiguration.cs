using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class DriverReportReasonConfiguration : IEntityTypeConfiguration<DriverReportReason>
    {
        public void Configure(EntityTypeBuilder<DriverReportReason> builder)
        {
            builder.HasKey(dr => new { dr.DriverReportId, dr.ReportReasonId });

            builder.HasOne(dr => dr.DriverReport)
                .WithMany(r => r.Reasons)
                .HasForeignKey(dr => dr.DriverReportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dr => dr.ReportReason)
                .WithMany(rr => rr.DriverReports)
                .HasForeignKey(dr => dr.ReportReasonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
