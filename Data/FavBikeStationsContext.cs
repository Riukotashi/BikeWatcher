using BikeWatcher.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeWatcher.Data
{
    public class FavBikeStationsContext : DbContext
    {
        public FavBikeStationsContext(DbContextOptions<FavBikeStationsContext> options) : base(options)
        {
        }

        public DbSet<FavBikeStations> FavBikeStations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavBikeStations>().ToTable("FavBikeStation");
        }
    }
}
