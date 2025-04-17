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
using EMGADSB.ViewModels;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMGADSB.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CarsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Cars
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, int? makeId, int? modelId)
        {
            var viewModel = new CarSearchViewModel
            {
                SearchString = searchString,
                MakeId = makeId,
                ModelId = modelId,
                Makes = await _context.CarMakes.ToListAsync(),
                Models = await _context.CarModels.ToListAsync()
            };

            var cars = _context.Cars
                .Include(c => c.CarMake)
                .Include(c => c.CarModel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Name.Contains(searchString) ||
                                       c.Description.Contains(searchString));
            }

            if (makeId.HasValue)
            {
                cars = cars.Where(c => c.CarMakeId == makeId.Value);
            }

            if (modelId.HasValue)
            {
                cars = cars.Where(c => c.CarModelId == modelId.Value);
            }

            viewModel.Cars = await cars.ToListAsync();

            return View(viewModel);
        }

        // GET: Cars/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarMake)
                .Include(c => c.CarModel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateCarViewModel
            {
                AvailableMakes = await _context.CarMakes.ToListAsync(),
                AvailableModels = await _context.CarModels.ToListAsync()
            };
            return View(viewModel);
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCarViewModel viewModel)
        {
            // Vérification manuelle si CarModelId est valide pour le CarMakeId sélectionné
            if (viewModel.CarMakeId > 0 && viewModel.CarModelId > 0)
            {
                var modelBelongsToMake = await _context.CarModels
                    .AnyAsync(m => m.Id == viewModel.CarModelId && m.CarMakeId == viewModel.CarMakeId);

                if (!modelBelongsToMake)
                {
                    ModelState.AddModelError("CarModelId", "Le modèle sélectionné n'appartient pas à la marque choisie.");
                }
            }

            if (ModelState.IsValid)
            {
                var car = new Car
                {
                    Name = viewModel.Name,
                    Year = viewModel.Year,
                    Price = viewModel.Price,
                    Description = viewModel.Description,
                    CarMakeId = viewModel.CarMakeId,
                    CarModelId = viewModel.CarModelId,
                    IsAvailable = true,
                    IsSold = false,
                    DateAdded = DateTime.Now
                };

                if (viewModel.Image != null && viewModel.Image.Length > 0)
                {
                    // Création du dossier s'il n'existe pas
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images/cars");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Image.CopyToAsync(fileStream);
                    }
                    car.ImageUrl = "/images/cars/" + uniqueFileName;
                }
                else
                {
                    // Image par défaut si aucune n'est fournie
                    car.ImageUrl = "/images/cars/default-car.jpg";
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si on arrive ici, c'est que la validation a échoué, donc on réinitialise les listes
            viewModel.AvailableMakes = await _context.CarMakes.ToListAsync();
            viewModel.AvailableModels = await _context.CarModels.ToListAsync();
            return View(viewModel);
        }

        // GET: Cars/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarMake)
                .Include(c => c.CarModel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new CarEditViewModel
            {
                Id = car.Id,
                Name = car.Name,
                Year = car.Year,
                Price = car.Price,
                Description = car.Description,
                CurrentImageUrl = car.ImageUrl,
                CarMakeId = car.CarMakeId,
                CarModelId = car.CarModelId,
                IsAvailable = car.IsAvailable,
                IsSold = car.IsSold,
                AvailableMakes = await _context.CarMakes.ToListAsync(),
                AvailableModels = await _context.CarModels.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CarEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Supprimons toute validation pour l'image si on conserve l'image actuelle
            if (viewModel.KeepCurrentImage)
            {
                ModelState.Remove("NewImage");
                ModelState.Remove("Image"); // Au cas où le nom du champ dans la vue serait encore "Image"
            }

            // Vérification manuelle si CarModelId est valide pour le CarMakeId sélectionné
            if (viewModel.CarMakeId > 0 && viewModel.CarModelId > 0)
            {
                var modelBelongsToMake = await _context.CarModels
                    .AnyAsync(m => m.Id == viewModel.CarModelId && m.CarMakeId == viewModel.CarMakeId);

                if (!modelBelongsToMake)
                {
                    ModelState.AddModelError("CarModelId", "Le modèle sélectionné n'appartient pas à la marque choisie.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var car = await _context.Cars.FindAsync(id);
                    if (car == null)
                    {
                        return NotFound();
                    }

                    car.Name = viewModel.Name;
                    car.Year = viewModel.Year;
                    car.Price = viewModel.Price;
                    car.Description = viewModel.Description;
                    car.CarMakeId = viewModel.CarMakeId ?? 0;
                    car.CarModelId = viewModel.CarModelId ?? 0;
                    car.IsAvailable = viewModel.IsAvailable;
                    car.IsSold = viewModel.IsSold;

                    // Traitement de l'image
                    if (viewModel.NewImage != null && viewModel.NewImage.Length > 0)
                    {
                        // Supprimer l'ancienne image si nécessaire
                        if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.EndsWith("default-car.jpg"))
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, car.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Sauvegarder la nouvelle image
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "images/cars");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.NewImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.NewImage.CopyToAsync(fileStream);
                        }

                        car.ImageUrl = "/images/cars/" + uniqueFileName;
                    }
                    else if (!viewModel.KeepCurrentImage)
                    {
                        // Si l'utilisateur ne veut pas garder l'image actuelle
                        car.ImageUrl = "/images/cars/default-car.jpg";
                    }
                    // Sinon, on garde l'image actuelle

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // La validation a échoué, on recharge les données pour le formulaire
            viewModel.AvailableMakes = await _context.CarMakes.ToListAsync();
            viewModel.AvailableModels = await _context.CarModels.ToListAsync();
            return View(viewModel);
        }

        // GET: Cars/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarMake)
                .Include(c => c.CarModel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            // Supprimer l'image associée si elle existe
            if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.EndsWith("default-car.jpg"))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, car.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Cars/MarkAsSold/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsSold(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.IsSold = true;
            car.IsAvailable = false;
            car.DateSold = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Cars/MarkAsUnavailable/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsUnavailable(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.IsAvailable = false;
            car.IsSold = false;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Cars/MarkAsAvailable/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsAvailable(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.IsAvailable = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Cars/ToggleAvailability/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.IsAvailable = !car.IsAvailable;

            // Si marqué comme non disponible, il ne peut pas être vendu
            if (!car.IsAvailable)
            {
                car.IsSold = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: Cars/GetModelsByMake/5
        [HttpGet]
        public async Task<JsonResult> GetModelsByMake(int makeId)
        {
            var models = await _context.CarModels
                .Where(m => m.CarMakeId == makeId)
                .Select(m => new { id = m.Id, name = m.Name })
                .ToListAsync();

            return Json(models);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}