using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;
namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class QueueConfiguration : IEntityTypeConfiguration<Queue>
    {
        public void Configure(EntityTypeBuilder<Queue> builder)
        {
            builder.HasKey(q => q.Id);

            builder.HasOne(q => q.Station)
                .WithMany(r => r.Queues)
                .HasForeignKey(q => q.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Route)
                .WithMany(r => r.Queues)
                .HasForeignKey(q => q.RouteId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(q => new { q.StationId, q.RouteId }).IsUnique();
        }
    }
}
