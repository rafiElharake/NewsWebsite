namespace y.Models
{
    public class NewsResponse
    {
        public string Status { get; set; }
        public int totalArticles { get; set; }
        public List<Article> Articles { get; set; }
        public PaginationViewModel Pagination { get; set; }
    }

}
