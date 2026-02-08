using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.LicenseNumber)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.HasOne(d => d.ApplicationUser)
                .WithOne()
                .HasForeignKey<Driver>(d => d.Id);
        }
    }
}
