using EMGADSB.Models;
using System.ComponentModel.DataAnnotations;

namespace EMGADSB.ViewModels
{
    // ViewModels/CarModelViewModel.cs
    public class CarModelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du modèle est requis")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La marque est requise")]
        public int CarMakeId { get; set; }

        public List<CarMake> AvailableMakes { get; set; }
    }
}
