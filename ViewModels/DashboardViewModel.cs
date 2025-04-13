using System.Collections.Generic;
using EMGADSB.Models;
using EMGADSB.Models;

namespace EMGADSB.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int SoldCars { get; set; }
        public int TotalCarMakes { get; set; }
        public List<Car> RecentlyAddedCars { get; set; }
        public List<Car> RecentlySoldCars { get; set; }
    }
}
