namespace EMGADSB.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; } // Marque
        public string Model { get; set; } // Modèle
        public int Year { get; set; } // Année
        public decimal PurchasePrice { get; set; } // Prix d'achat
        public decimal SellingPrice { get; set; } // Prix de vente
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsSold { get; set; } = false;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateSold { get; set; }

        // Navigation properties
        public int CarMakeId { get; set; }
        public CarMake CarMakeNavigation { get; set; }
    }

}