using EMGADSB.Models;
using System.Collections.Generic;

namespace EMGADSB.ViewModels
{
    public class ManageMakesModelsViewModel
    {
        public List<CarMake> Makes { get; set; } = new List<CarMake>();
        public List<CarModel> Models { get; set; } = new List<CarModel>();

        // Pour l'ajout d'un nouveau modèle
        public int SelectedMakeId { get; set; }
        public string NewModelName { get; set; }

        // Pour l'ajout d'une nouvelle marque
        public string NewMakeName { get; set; }
    }
}