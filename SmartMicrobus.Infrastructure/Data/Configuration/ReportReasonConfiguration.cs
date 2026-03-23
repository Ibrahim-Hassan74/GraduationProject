using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class ReportReasonConfiguration : IEntityTypeConfiguration<ReportReason>
    {
        public void Configure(EntityTypeBuilder<ReportReason> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.NameAr).IsRequired().HasMaxLength(200);
            builder.Property(r => r.NameEn).IsRequired().HasMaxLength(200);
        }
    }
}
