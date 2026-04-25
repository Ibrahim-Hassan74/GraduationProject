

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class MicrobusConfiguration : IEntityTypeConfiguration<Microbus>
    {
        public void Configure(EntityTypeBuilder<Microbus> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(m => m.PlateNumber).IsUnique();

            builder.HasOne(m => m.Route)
                .WithMany(r => r.Microbuses)
                .HasForeignKey(m => m.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
