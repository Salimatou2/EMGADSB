using EMGADSB.Models;

namespace EMGADSB.ViewModels
{
    public class CarSearchViewModel
    {
        public List<Car> Cars { get; set; }
        public string SearchString { get; set; }
        public int? MakeId { get; set; }
        public int? ModelId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<CarMake> Makes { get; set; }
        public List<CarModel> Models { get; set; }
    }
}
