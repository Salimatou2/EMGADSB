using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMGADSB.ViewModels
{
    //public class EditUserRolesViewModel
    //{
    //    public string Id { get; set; }

    //    [Required(ErrorMessage = "L'adresse email est requise")]
    //    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    //    public string Email { get; set; }

    //    [Display(Name = "Rôles")]
    //    public List<string> SelectedRoles { get; set; }
    //}

    // ViewModels pour la gestion des rôles
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<UserRoleViewModel> Roles { get; set; } = new List<UserRoleViewModel>();
    }
}
