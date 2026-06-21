using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
    {
        public void Configure(EntityTypeBuilder<Manager> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.ApplicationUser)
                .WithOne()
                .HasForeignKey<Manager>(m => m.Id);

            builder.HasOne(m => m.Station)
                .WithMany(s => s.Managers)
                .HasForeignKey(m => m.StationId);
        }
    }
}
