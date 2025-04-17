using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EMGADSB.Models;
using EMGADSB.Services;
using System.Threading.Tasks;
using System.Linq;
using EMGADSB.Data;
using EMGADSB.Services;

namespace EMGOXD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtSettings jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtService;
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtSettings.GenerateToken(user.Id, user.UserName, roles.ToList());

            return Ok(new { token });
        }
    }
}
