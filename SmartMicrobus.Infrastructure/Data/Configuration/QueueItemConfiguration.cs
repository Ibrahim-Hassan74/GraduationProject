
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class QueueItemConfiguration: IEntityTypeConfiguration<QueueItem>
    {
        public void Configure(EntityTypeBuilder<QueueItem> builder)
        {
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Position)
                .IsRequired();

            builder.Property(q => q.Status)
                .IsRequired();

            builder.Property(q => q.JoinedAt)
                .IsRequired();

            builder.HasOne(q => q.Queue)
                .WithMany(q => q.Items)
                .HasForeignKey(q => q.QueueId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Driver)
                .WithMany()
                .HasForeignKey(q => q.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(q => new { q.QueueId, q.Position })
                .IsUnique();
        }
    }
}
