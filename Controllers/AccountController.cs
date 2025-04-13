using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using EMGADSB.Models;
using EMGADSB.ViewModels;
using EMGADSB.Data;

namespace EMGADSB.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Recherche de l'utilisateur par email
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Vérification si l'utilisateur existe
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Utilisateur non trouvé.");
                    return View(model);
                }

                // Vérification des rôles de l'utilisateur
                var roles = await _userManager.GetRolesAsync(user);
                var isInAdminRole = roles.Contains("Admin");

                // Si l'utilisateur est admin, vérifie qu'il fait bien partie du rôle "Admin"
                if (!isInAdminRole && await _roleManager.RoleExistsAsync("Admin"))
                {
                    Console.WriteLine("Le rôle Admin existe mais l'utilisateur n'en fait pas partie.");
                }

                // Tentative de connexion avec les informations fournies
                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: model.RememberMe, // Gestion du cookie de session
                    lockoutOnFailure: false
                );

                // Si la connexion a réussi
                if (result.Succeeded)
                {
                    if (isInAdminRole)
                    {
                        // Redirection vers le tableau de bord Admin
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        // Redirection vers la page d'accueil
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Affichage du message d'erreur si la connexion échoue
                    string errorMessage = $"Échec de la connexion. Email: {model.Email}, " +
                                          $"Utilisateur trouvé: {user != null}, " +
                                          $"Est Admin: {isInAdminRole}, " +
                                          $"Rôles: {string.Join(", ", roles)}";

                    Console.WriteLine(errorMessage);

                    // Ajout de l'erreur au modèle pour l'afficher à l'utilisateur
                    ModelState.AddModelError(string.Empty, "Identifiants invalides.");
                }
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
