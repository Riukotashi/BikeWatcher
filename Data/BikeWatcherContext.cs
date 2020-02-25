using BikeWatcher.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeWatcher.Data
{
    public class BikeWatcherContext : DbContext
    {
        public BikeWatcherContext(DbContextOptions<BikeWatcherContext> options) : base(options)
        {
        }

        public DbSet<FavBikeStations> FavBikeStations { get; set; }
        public DbSet<AlertBike> AlertBike { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavBikeStations>().ToTable("FavBikeStation");
            modelBuilder.Entity<AlertBike>().ToTable("AlertBike");
        }
    }
}
