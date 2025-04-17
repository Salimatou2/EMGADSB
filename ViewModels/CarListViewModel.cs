using EMGADSB.Models;

namespace EMGADSB.ViewModels
{
    // ViewModels/CarListViewModel.cs
    public class CarListViewModel
    {
        public List<Car> Cars { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string NameSort { get; set; }
        public string PriceSort { get; set; }
        public string YearSort { get; set; }
        public bool ShowOnlyAvailable { get; set; }
    }
}
