using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMGADSB.Data;
using EMGADSB.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using EMGADSB.Data;
using EMGADSB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMGADSB.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CarsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars
                .Include(c => c.CarMakeNavigation)
                .ToListAsync();

            return View(cars);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarMakeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CarMakes = new SelectList(_context.CarMakes, "Id", "Name");
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CarMakeId,Model,Year,PurchasePrice,SellingPrice,Description")] Car car, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Vérifie que l'année est >= 2018
                if (car.Year < 2018)
                {
                    ModelState.AddModelError("Year", "L'année doit être 2018 ou plus récente.");
                    ViewBag.CarMakes = new SelectList(_context.CarMakes, "Id", "Name", car.CarMakeId);
                    return View(car);
                }

                // Traitement de l'image si elle existe
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/cars");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    car.ImageUrl = "/images/cars/" + fileName;
                }

                car.DateAdded = DateTime.Now;
                car.IsAvailable = true;
                car.IsSold = false;

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CarMakes = new SelectList(_context.CarMakes, "Id", "Name", car.CarMakeId);
            return View(car);
        }

        // GET: Cars/Edit/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            ViewBag.CarMakes = new SelectList(_context.CarMakes, "Id", "Name", car.CarMakeId);
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarMakeId,Model,Year,PurchasePrice,SellingPrice,Description,IsAvailable,IsSold,ImageUrl")] Car car, IFormFile ImageFile)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalCar = await _context.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    car.DateAdded = originalCar.DateAdded;
                    car.DateSold = originalCar.DateSold;

                    if (car.Year < 2018)
                    {
                        ModelState.AddModelError("Year", "L'année doit être 2018 ou plus récente.");
                        ViewBag.CarMakes = new SelectList(_context.CarMakes, "Id", "Name", car.CarMakeId);
                        return View(car);
                    }

                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(car.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, car.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/cars");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        car.ImageUrl = "/images/cars/" + fileName;
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CarMakes = new SelectList(_context.CarMakes, "Id", "Name", car.CarMakeId);
            return View(car);
        }

        // GET: Cars/MarkAsSold/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsSold(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarMakeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/MarkAsSold/5
        [HttpPost, ActionName("MarkAsSold")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsSoldConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                car.IsSold = true;
                car.IsAvailable = false;
                car.DateSold = DateTime.Now;

                _context.Update(car);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
