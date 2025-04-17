using System;
using System.Collections.Generic;
using EMGADSB.Models;

namespace EMGADSB.ViewModels
{
    public class StatisticsViewModel
    {
        // Statistiques générales
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int SoldCars { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePrice => SoldCars > 0 ? TotalRevenue / SoldCars : 0;

        // Statistiques mensuelles
        public Dictionary<string, int> MonthlySales { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> MonthlyRevenue { get; set; } = new Dictionary<string, decimal>();

        // Statistiques par marque
        public Dictionary<string, int> SalesByMake { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByMake { get; set; } = new Dictionary<string, decimal>();

        // Statistiques par modèle (top 5)
        public Dictionary<string, int> TopSellingModels { get; set; } = new Dictionary<string, int>();

        // Temps moyen de vente (en jours)
        public double AverageDaysToSell { get; set; }

        // Voitures récemment vendues
        public List<Car> RecentlySoldCars { get; set; } = new List<Car>();
    }
}