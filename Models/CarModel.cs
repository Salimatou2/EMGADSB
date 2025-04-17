namespace EMGADSB.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relations
        public int CarMakeId { get; set; }
        public CarMake CarMake { get; set; }

        // Navigation property
        public ICollection<Car> Cars { get; set; }
    }
}
