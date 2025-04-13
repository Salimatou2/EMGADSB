namespace EMGADSB.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public int CarMakeId { get; set; }
        public CarMake CarMakeNavigation { get; set; }
    }
}
