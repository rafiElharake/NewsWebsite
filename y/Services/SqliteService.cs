using Dapper;
using System.Data;
using y.Models;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Security;

namespace y.Services
{
    public class SQLiteService
    {
        private readonly IDbConnection _dbConnection; 

        public SQLiteService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection; 
        }
 
        public async Task AddFavoriteArticleAsync(int userId, string articleUrl, string title)
        {  
            var command = _dbConnection.CreateCommand();
            command.CommandText = "INSERT INTO SavedArticles (UserId, ArticleUrl, Title) VALUES (@userId, @articleUrl, @title)";
            await _dbConnection.ExecuteAsync(command.CommandText, new
            {
                userId,  
                articleUrl,
                title
            });
        }
        public async Task RemoveFavoriteArticleAsync(string articleUrl)
        {
            var command = _dbConnection.CreateCommand();
            command.CommandText = "DELETE FROM SavedArticles WHERE ArticleUrl = @articleUrl";

            await _dbConnection.ExecuteAsync(command.CommandText, new
            {
                articleUrl
            });
        }
        public async Task<string> GetSavedArticlesAsync(MemberIdentityUser user)
        { 
            var userId = user.Id;  
            var query = "SELECT ArticleUrl AS Url, Title FROM SavedArticles WHERE UserId = @UserId";
              var articles = await _dbConnection.QueryAsync<Article>(query, new { UserId = userId });
              foreach (var article in articles)
            {
                article.SourceName = string.Empty;
                article.Author = string.Empty;
                article.Description = string.Empty;
                article.Image = string.Empty;
                article.PublishedAt = default(DateTime);
                article.Content = string.Empty;
            } 
            var resultObject = new { Username = user.UserName, Articles = articles }; 
            var jsonResult = JsonConvert.SerializeObject(resultObject, Formatting.Indented);
             return jsonResult;
        }

    }
}