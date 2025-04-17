using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EMGADSB.Models;
using EMGADSB.Data;
using EMGADSB.Models;
using EMGADSB.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EMGADSB.Controllers
{
    // Controllers/HomeController.cs
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                FeaturedCars = await _context.Cars
                    .Include(c => c.CarMake)
                    .Include(c => c.CarModel)
                    .Where(c => c.IsAvailable && !c.IsSold)
                    .OrderByDescending(c => c.Price)
                    .Take(3)
                    .ToListAsync(),

                RecentlyAddedCars = await _context.Cars
                    .Include(c => c.CarMake)
                    .Include(c => c.CarModel)
                    .Where(c => c.IsAvailable && !c.IsSold)
                    .OrderByDescending(c => c.Id)
                    .Take(6)
                    .ToListAsync(),

                TotalCarsCount = await _context.Cars.CountAsync(),
                SoldCarsCount = await _context.Cars.Where(c => c.IsSold).CountAsync(),
                AvailableCarsCount = await _context.Cars.Where(c => c.IsAvailable && !c.IsSold).CountAsync()
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
