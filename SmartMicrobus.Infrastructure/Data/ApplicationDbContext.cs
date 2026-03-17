using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using System.Reflection;

namespace SmartMicrobus.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Passenger> Passangers { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }

        public virtual DbSet<Microbus> Microbuses { get; set; }

        public virtual DbSet<Station> Stations { get; set; }

        public virtual DbSet<Route> Routes { get; set; }

        public virtual DbSet<Queue> Queues { get; set; }

        public virtual DbSet<QueueItem> QueueItems { get; set; }

        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<FavoriteRoute> FavoriteRoutes { get; set; }
    }   
}
