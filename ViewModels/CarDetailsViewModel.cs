namespace EMGADSB.ViewModels
{
    // ViewModels/CarDetailsViewModel.cs
    public class CarDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsSold { get; set; }
        public string CarMakeName { get; set; }
        public string CarModelName { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
