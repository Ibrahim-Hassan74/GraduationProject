
using SmartMicrobus.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.FromAr)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.ToAr)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.FromEn)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.ToEn)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Price)
                .HasColumnType("decimal(10,2)");

            builder.HasOne(r => r.FromStation)
                .WithMany(s => s.FromRoutes)
                .HasForeignKey(r => r.FromStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ToStation)
                .WithMany(s => s.ToRoutes)
                .HasForeignKey(r => r.ToStationId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.HasIndex(r => new { r.FromEn, r.ToEn })
            //    .IsUnique();
            //builder.HasIndex(r => new { r.FromAr, r.ToAr })
            //    .IsUnique();
            builder.HasIndex(r => new { r.FromStationId, r.ToStationId })
                .IsUnique();
        }
    }
}
