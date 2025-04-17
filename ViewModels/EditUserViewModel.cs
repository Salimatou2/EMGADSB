namespace EMGADSB.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }      // L'ID de l'utilisateur (peut être utilisé pour l'identification de l'utilisateur à modifier)
        public string UserName { get; set; } // Le nom d'utilisateur
        public string Email { get; set; }    // L'email de l'utilisateur
        public string FullName { get; set; } // Le nom complet de l'utilisateur (facultatif, à ajouter si nécessaire)

        // Si tu veux d'autres informations pour l'édition, comme un mot de passe, tu peux les ajouter :
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
