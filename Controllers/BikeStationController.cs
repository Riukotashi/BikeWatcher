using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Text.Json;
using BikeWatcher.Models;
using System.Collections.Generic;
using BikeWatcher.Data;
using Microsoft.EntityFrameworkCore;

namespace BikeWatcher.Controllers
{
    public class BikeStationController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly BikeWatcherContext _context;

        public BikeStationController(BikeWatcherContext context)
        {
            _context = context;
        }

        static async Task<List<BikeStation>> GetBikeStationsAsync(string city)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            if (city.ToLower() == "bordeaux")
            {
                var streamTaskBdx = client.GetStreamAsync("https://api.alexandredubois.com/vcub-backend/vcub.php");
                var bikeStationsBdx = await JsonSerializer.DeserializeAsync<List<BikeStationBdx>>(await streamTaskBdx);
                var bikeStations = new List<BikeStation>();
                foreach (var bikeStationBdx in bikeStationsBdx)
                {
                    var bikeStation = new BikeStation(bikeStationBdx);
                    bikeStations.Add(bikeStation);
                }
                return bikeStations;
            }

            var streamTask = client.GetStreamAsync("https://download.data.grandlyon.com/ws/rdata/jcd_jcdecaux.jcdvelov/all.json");
            var bikeStationJson = await JsonSerializer.DeserializeAsync<BikeStationJson>(await streamTask);
            return bikeStationJson.values;

        }

        public async Task<IActionResult> Index(string city = "lyon")
        {
            var bikeStations = await GetBikeStationsAsync(city);
            ViewBag.bikeStations = bikeStations.OrderBy(x => x.name);
            return View();
        }

        public async Task<IActionResult> MapAsync(string city = "lyon")
        {
            var bikeStations = await GetBikeStationsAsync(city);
            ViewBag.bikeStations = bikeStations;
            return View();
        }

        public async Task<IActionResult> AddToFav(int id)
        {
            var favBikeStation = new FavBikeStations();
            favBikeStation.idFav = id;
            _context.FavBikeStations.Add(favBikeStation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        public IActionResult AlertBike()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AlertBike([Bind("numberBike, message")] AlertBike alertBike)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(alertBike);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(alertBike);
        }

        public async Task<IActionResult> FavoriteAsync()
        {
            return View(await _context.FavBikeStations.ToListAsync());
        }
    }
}