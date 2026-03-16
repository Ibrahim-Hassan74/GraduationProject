using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class FavoriteRouteConfiguration : IEntityTypeConfiguration<FavoriteRoute>
    {
        public void Configure(EntityTypeBuilder<FavoriteRoute> builder)
        {
            builder.HasIndex(x => new { x.PassengerId, x.RouteId }).IsUnique();
        }
    }
}
