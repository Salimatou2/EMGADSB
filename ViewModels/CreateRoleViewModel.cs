using System.ComponentModel.DataAnnotations;

namespace EMGADSB.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Le nom du rôle est requis")]
        [Display(Name = "Nom du rôle")]
        public string Name { get; set; }
    }
}
