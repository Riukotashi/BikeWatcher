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

        private readonly FavBikeStationsContext _context;

        public BikeStationController(FavBikeStationsContext context)
        {
            _context = context;
        }

        static async Task<List<BikeStation>> GetBikeStationsAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var streamTask = client.GetStreamAsync("https://download.data.grandlyon.com/ws/rdata/jcd_jcdecaux.jcdvelov/all.json");
            var bikeStationJson = await JsonSerializer.DeserializeAsync<BikeStationJson>(await streamTask);
            return bikeStationJson.values;

        }

        public async Task<IActionResult> Index()
        {
            var bikeStations = await GetBikeStationsAsync();
            ViewBag.bikeStations = bikeStations.OrderBy(x => x.name);
            return View();
        }

        public async Task<IActionResult> MapAsync()
        {
            var bikeStations = await GetBikeStationsAsync();
            ViewBag.bikeStations = bikeStations;
            return View();
        }

        public async Task<IActionResult> AddToFav(int id)
        {
            if (id is int)
            {
                var favBikeStation = new FavBikeStations();
                favBikeStation.idFav = id;
                _context.FavBikeStations.Add(favBikeStation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
            
        }

        public async Task<IActionResult> FavoriteAsync()
        {
            return View(await _context.FavBikeStations.ToListAsync());
        }
    }
}