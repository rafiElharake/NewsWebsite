using y.Models;

public class SavedArticle
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ArticleUrl { get; set; }
     
    public Users User { get; set; }
}
