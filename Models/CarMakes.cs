namespace EMGADSB.Models
{
    public class CarMake
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Car> Cars { get; set; }
        public ICollection<CarModel> CarModels { get; set; }
    }



}
