namespace y.Models
{
    public class PaginationViewModel
    {
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
        public int TotalPages { get; set; }
        public string PaginationUrlFormat { get; set; }
        public int CurrentPage { get; set; }
    }
}
