using System.Collections.Generic;
using EMGADSB.Models;

namespace EMGADSB.ViewModels
{
    // ViewModels/DashboardViewModel.cs
    public class DashboardViewModel
    {
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int SoldCars { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Car> RecentlyAddedCars { get; set; }
        public List<Car> RecentlySoldCars { get; set; }
    }
}
