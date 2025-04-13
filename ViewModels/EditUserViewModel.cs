using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMGADSB.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "L'adresse email est requise")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; }

        [Display(Name = "Rôles")]
        public List<string> SelectedRoles { get; set; }
    }
}
