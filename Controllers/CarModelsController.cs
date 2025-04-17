using EMGADSB.Data;
using EMGADSB.Models;
using EMGADSB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMGADSB.Controllers
{
    // Controllers/CarModelsController.cs
    [Authorize(Roles = "Admin")]
    public class CarModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CarModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.CarModels
                .Include(m => m.CarMake)
                .ToListAsync());
        }

        // GET: CarModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels
                .Include(m => m.CarMake)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carModel == null)
            {
                return NotFound();
            }

            return View(carModel);
        }

        // GET: CarModels/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CarModelViewModel
            {
                AvailableMakes = await _context.CarMakes.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: CarModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarModelViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var carModel = new CarModel
                {
                    Name = viewModel.Name,
                    CarMakeId = viewModel.CarMakeId
                };

                _context.Add(carModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableMakes = await _context.CarMakes.ToListAsync();
            return View(viewModel);
        }

        // GET: CarModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels.FindAsync(id);
            if (carModel == null)
            {
                return NotFound();
            }

            var viewModel = new CarModelViewModel
            {
                Id = carModel.Id,
                Name = carModel.Name,
                CarMakeId = carModel.CarMakeId,
                AvailableMakes = await _context.CarMakes.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: CarModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarModelViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var carModel = await _context.CarModels.FindAsync(id);
                    if (carModel == null)
                    {
                        return NotFound();
                    }

                    carModel.Name = viewModel.Name;
                    carModel.CarMakeId = viewModel.CarMakeId;

                    _context.Update(carModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarModelExists(viewModel.Id))
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

            viewModel.AvailableMakes = await _context.CarMakes.ToListAsync();
            return View(viewModel);
        }

        // GET: CarModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels
                .Include(m => m.CarMake)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carModel == null)
            {
                return NotFound();
            }

            return View(carModel);
        }

        // POST: CarModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Vérifier si des voitures utilisent ce modèle
            var hasRelatedCars = await _context.Cars.AnyAsync(c => c.CarModelId == id);

            if (hasRelatedCars)
            {
                ModelState.AddModelError(string.Empty, "Ce modèle ne peut pas être supprimé car il est utilisé par des voitures.");
                var carModel = await _context.CarModels
                    .Include(m => m.CarMake)
                    .FirstOrDefaultAsync(m => m.Id == id);
                return View(carModel);
            }

            var carModelToDelete = await _context.CarModels.FindAsync(id);
            if (carModelToDelete != null)
            {
                _context.CarModels.Remove(carModelToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarModelExists(int id)
        {
            return _context.CarModels.Any(e => e.Id == id);
        }
    }
}
