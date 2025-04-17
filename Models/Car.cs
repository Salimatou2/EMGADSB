using System;
using System.ComponentModel.DataAnnotations;

namespace EMGADSB.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        public string Name { get; set; }

        [Required]
        [Range(2018, 2100, ErrorMessage = "L'année doit être supérieure ou égale à 2018")]
        public int Year { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsSold { get; set; } = false;

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public DateTime? DateSold { get; set; }

        // Relations
        [Required]
        public int CarMakeId { get; set; }
        public CarMake CarMake { get; set; }

        [Required]
        public int CarModelId { get; set; }
        public CarModel CarModel { get; set; }
    }
}