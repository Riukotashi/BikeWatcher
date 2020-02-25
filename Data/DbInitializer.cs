using BikeWatcher.Models;
using BikeWatcher.Data;
using System;
using System.Linq;

namespace BikeWatcher.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FavBikeStationsContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.FavBikeStations.Any())
            {
                return;   // DB has been seeded
            }

            var favorises = new FavBikeStations[]
            {
            new FavBikeStations{idFav=16005},
            new FavBikeStations{idFav=16045}

            };
            foreach (FavBikeStations f in favorises)
            {
                context.FavBikeStations.Add(f);
            }
            context.SaveChanges();

        }
    }
}