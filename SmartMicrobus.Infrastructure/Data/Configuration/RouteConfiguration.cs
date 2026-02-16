
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

            builder.Property(r => r.From)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.To)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Price)
                .HasColumnType("decimal(10,2)");

            builder.HasOne(r => r.Station)
                .WithMany(s => s.Routes)
                .HasForeignKey(r => r.StationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
