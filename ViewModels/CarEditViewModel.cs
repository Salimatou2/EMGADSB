using EMGADSB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EMGADSB.ViewModels
{
    public class CarEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        public string Name { get; set; }

        [Required(ErrorMessage = "L'année est requise")]
        [Range(2018, 2100, ErrorMessage = "L'année doit être supérieure ou égale à 2018")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Le prix est requis")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La description est requise")]
        public string Description { get; set; }

        public string CurrentImageUrl { get; set; }

        public IFormFile NewImage { get; set; }

        public bool KeepCurrentImage { get; set; } = true;

        [Required(ErrorMessage = "La marque est requise")]
        public int? CarMakeId { get; set; }

        [Required(ErrorMessage = "Le modèle est requis")]
        public int? CarModelId { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsSold { get; set; }

        [Display(Name = "Date d'ajout")]
        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        [Display(Name = "Date de vente")]
        [DataType(DataType.Date)]
        public DateTime? DateSold { get; set; }

        public List<CarMake> AvailableMakes { get; set; } = new List<CarMake>();

        public List<CarModel> AvailableModels { get; set; } = new List<CarModel>();
    }
}