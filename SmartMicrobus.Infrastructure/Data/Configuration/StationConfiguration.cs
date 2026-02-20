

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.NameAr)
                .IsRequired()
                .HasMaxLength(200);


            builder.Property(s => s.CityEn)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.CityAr)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
