using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EMGADSB.Models;
using EMGADSB.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace EMGADSB.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var role = model.Email == "admin@emgoasb.com" ? "Admin" : "User";
                    await _userManager.AddToRoleAsync(user, role);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> InitializeRoles()
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // ✅ TA MÉTHODE AJOUTÉE ICI :
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@emgoasb.com",
                Email = "admin@emgoasb.com",
                FirstName = "Admin",
                LastName = "Principal"
            };

            var userExists = await _userManager.FindByEmailAsync(admin.Email);

            if (userExists == null)
            {
                var result = await _userManager.CreateAsync(admin, "Admin123!");

                if (result.Succeeded)
                {
                    // ➕ Assurer l'existence du rôle avant de l'assigner
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    await _userManager.AddToRoleAsync(admin, "Admin");

                    ViewBag.Message = "Administrateur créé avec succès. Email: admin@emgoasb.com, Mot de passe: Admin123!";
                }
                else
                {
                    ViewBag.Message = "Échec de la création: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                ViewBag.Message = "Un administrateur existe déjà avec cet email.";
            }

            return View("AdminCreated");
        }

    }
}
