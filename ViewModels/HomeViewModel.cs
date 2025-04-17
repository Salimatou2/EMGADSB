using EMGADSB.Models;

namespace EMGADSB.ViewModels
{
    // ViewModels/HomeViewModel.cs
    public class HomeViewModel
    {
        public List<Car> FeaturedCars { get; set; }
        public List<Car> RecentlyAddedCars { get; set; }
        public int TotalCarsCount { get; set; }
        public int SoldCarsCount { get; set; }
        public int AvailableCarsCount { get; set; }
    }
}
