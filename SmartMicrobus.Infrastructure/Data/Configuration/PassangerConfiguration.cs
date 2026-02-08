using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMicrobus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Infrastructure.Data.Configuration
{
    public class PassangerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasKey(p => p.PassengerId);
            builder.HasOne(p => p.ApplicationUser)
                .WithOne()
                .HasForeignKey<Passenger>(p => p.PassengerId);
        }
    }
}
