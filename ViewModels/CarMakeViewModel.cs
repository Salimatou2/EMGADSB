using System.ComponentModel.DataAnnotations;

namespace EMGADSB.ViewModels
{
    // ViewModels/CarMakeViewModel.cs
    public class CarMakeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la marque est requis")]
        public string Name { get; set; }
    }

}
