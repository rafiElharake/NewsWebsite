namespace y.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public List<SavedArticle> SavedArticles { get; set; }

    }
}
