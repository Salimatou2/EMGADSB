namespace EMGADSB.ViewModels
{
    public class CarSearchViewModel
    {
        public int? CarMakeId { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool AvailableOnly { get; set; } = true;
        public string SortBy { get; set; } = "DateAdded";
        public string SortDirection { get; set; } = "Desc";
    }
}
