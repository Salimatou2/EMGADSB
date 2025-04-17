using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMGADSB.Data;
using EMGADSB.Models;
using EMGADSB.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EMGADSB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalCars = await _context.Cars.CountAsync(),
                AvailableCars = await _context.Cars.Where(c => c.IsAvailable && !c.IsSold).CountAsync(),
                SoldCars = await _context.Cars.Where(c => c.IsSold).CountAsync(),
                TotalRevenue = await _context.Cars
                    .Where(c => c.IsSold)
                    .SumAsync(c => c.Price),

                RecentlyAddedCars = await _context.Cars
                    .Include(c => c.CarMake)
                    .Include(c => c.CarModel)
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .ToListAsync(),

                RecentlySoldCars = await _context.Cars
                    .Include(c => c.CarMake)
                    .Include(c => c.CarModel)
                    .Where(c => c.IsSold)
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GESTION DES ROLES
        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.Name);
                if (!roleExists)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Roles));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ce rôle existe déjà.");
                }
            }

            return View(model);
        }

        // GESTION DES UTILISATEURS
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserRolesViewModel
            {
                UserId = userId,
                UserName = user.UserName
            };

            var roles = await _roleManager.Roles.ToListAsync();
            foreach (var role in roles)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, role.Name);
                model.Roles.Add(new UserRoleViewModel
                {
                    RoleName = role.Name,
                    IsSelected = isInRole
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            for (int i = 0; i < model.Roles.Count; i++)
            {
                var role = model.Roles[i];
                var isInRole = await _userManager.IsInRoleAsync(user, role.RoleName);

                if (role.IsSelected && !isInRole)
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
                else if (!role.IsSelected && isInRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                }
            }

            return RedirectToAction(nameof(Users));
        }

        // GESTION DES MARQUES ET MODÈLES
        // GET: Admin/ManageMakesModels
        public async Task<IActionResult> ManageMakesModels()
        {
            var viewModel = new ManageMakesModelsViewModel
            {
                Makes = await _context.CarMakes.Include(m => m.CarModels).ToListAsync(),
                Models = await _context.CarModels.Include(m => m.CarMake).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Admin/AddMake
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMake(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Vérifier si la marque existe déjà
                var existingMake = await _context.CarMakes.FirstOrDefaultAsync(m => m.Name == name);
                if (existingMake == null)
                {
                    var make = new CarMake { Name = name };
                    _context.CarMakes.Add(make);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"La marque {name} a été ajoutée avec succès.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"La marque {name} existe déjà.";
                }
            }

            return RedirectToAction(nameof(ManageMakesModels));
        }

        // POST: Admin/AddModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddModel(int makeId, string name)
        {
            if (makeId > 0 && !string.IsNullOrWhiteSpace(name))
            {
                // Vérifier si le modèle existe déjà pour cette marque
                var existingModel = await _context.CarModels
                    .FirstOrDefaultAsync(m => m.Name == name && m.CarMakeId == makeId);

                if (existingModel == null)
                {
                    var model = new CarModel { Name = name, CarMakeId = makeId };
                    _context.CarModels.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Le modèle {name} a été ajouté avec succès.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Le modèle {name} existe déjà pour cette marque.";
                }
            }

            return RedirectToAction(nameof(ManageMakesModels));
        }

        // POST: Admin/DeleteMake
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMake(int id)
        {
            var make = await _context.CarMakes.Include(m => m.CarModels).FirstOrDefaultAsync(m => m.Id == id);
            if (make != null)
            {
                // Vérifier si des voitures utilisent cette marque
                var carsUsingMake = await _context.Cars.AnyAsync(c => c.CarMakeId == id);
                if (carsUsingMake)
                {
                    TempData["ErrorMessage"] = $"Impossible de supprimer la marque {make.Name} car elle est utilisée par des voitures.";
                }
                else
                {
                    // Supprimer tous les modèles associés
                    _context.CarModels.RemoveRange(make.CarModels);
                    // Supprimer la marque
                    _context.CarMakes.Remove(make);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"La marque {make.Name} et ses modèles ont été supprimés avec succès.";
                }
            }

            return RedirectToAction(nameof(ManageMakesModels));
        }

        // POST: Admin/DeleteModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModel(int id)
        {
            var model = await _context.CarModels.Include(m => m.CarMake).FirstOrDefaultAsync(m => m.Id == id);
            if (model != null)
            {
                // Vérifier si des voitures utilisent ce modèle
                var carsUsingModel = await _context.Cars.AnyAsync(c => c.CarModelId == id);
                if (carsUsingModel)
                {
                    TempData["ErrorMessage"] = $"Impossible de supprimer le modèle {model.Name} car il est utilisé par des voitures.";
                }
                else
                {
                    _context.CarModels.Remove(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Le modèle {model.Name} a été supprimé avec succès.";
                }
            }

            return RedirectToAction(nameof(ManageMakesModels));
        }

        // GET: Admin/Statistics
        public async Task<IActionResult> Statistics()
        {
            // Créez un modèle de vue avec les statistiques que vous souhaitez afficher
            var statisticsViewModel = new StatisticsViewModel
            {
                // Ajoutez ici les données statistiques dont vous avez besoin
                TotalCars = await _context.Cars.CountAsync(),
                SoldCars = await _context.Cars.Where(c => c.IsSold).CountAsync(),
                TotalRevenue = await _context.Cars
                    .Where(c => c.IsSold)
                    .SumAsync(c => c.Price),
                // ... autres statistiques
            };

            return View(statisticsViewModel);
        }

    }
}