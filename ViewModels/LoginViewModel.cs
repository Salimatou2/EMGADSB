using System.ComponentModel.DataAnnotations;

namespace EMGADSB.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'adresse email est requise")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        // Ajoutez cette propriété
        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }
}
