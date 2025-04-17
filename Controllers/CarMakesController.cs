using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMGADSB.Data;
using EMGADSB.Models;
using EMGADSB.Data;
using EMGADSB.Models;
using EMGADSB.ViewModels;

namespace EMGADSB.Controllers
{
    // Controllers/CarMakesController.cs
    [Authorize(Roles = "Admin")]
    public class CarMakesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarMakesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CarMakes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CarMakes.ToListAsync());
        }

        // GET: CarMakes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carMake = await _context.CarMakes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carMake == null)
            {
                return NotFound();
            }

            return View(carMake);
        }

        // GET: CarMakes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarMakes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] CarMakeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var carMake = new CarMake
                {
                    Name = viewModel.Name
                };

                _context.Add(carMake);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: CarMakes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carMake = await _context.CarMakes.FindAsync(id);
            if (carMake == null)
            {
                return NotFound();
            }

            var viewModel = new CarMakeViewModel
            {
                Id = carMake.Id,
                Name = carMake.Name
            };

            return View(viewModel);
        }

        // POST: CarMakes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CarMakeViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var carMake = await _context.CarMakes.FindAsync(id);
                    if (carMake == null)
                    {
                        return NotFound();
                    }

                    carMake.Name = viewModel.Name;
                    _context.Update(carMake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarMakeExists(viewModel.Id))
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
            return View(viewModel);
        }

        // GET: CarMakes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carMake = await _context.CarMakes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carMake == null)
            {
                return NotFound();
            }

            return View(carMake);
        }

        // POST: CarMakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Vérifier si des voitures utilisent cette marque
            var hasRelatedCars = await _context.Cars.AnyAsync(c => c.CarMakeId == id);
            var hasRelatedModels = await _context.CarModels.AnyAsync(m => m.CarMakeId == id);

            if (hasRelatedCars || hasRelatedModels)
            {
                ModelState.AddModelError(string.Empty, "Cette marque ne peut pas être supprimée car elle est utilisée par des voitures ou des modèles.");
                var carMake = await _context.CarMakes.FindAsync(id);
                return View(carMake);
            }

            var carMakeToDelete = await _context.CarMakes.FindAsync(id);
            if (carMakeToDelete != null)
            {
                _context.CarMakes.Remove(carMakeToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarMakeExists(int id)
        {
            return _context.CarMakes.Any(e => e.Id == id);
        }
    }
}