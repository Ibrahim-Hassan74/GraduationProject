using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class DriverReportConfiguration : IEntityTypeConfiguration<DriverReport>
    {
        public void Configure(EntityTypeBuilder<DriverReport> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.PlateNumber)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasMany(r => r.Reasons)
                .WithOne(rr => rr.DriverReport)
                .HasForeignKey(rr => rr.DriverReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
