using Dapper;
using System.Data;
using y.Models;
using Newtonsoft.Json;

namespace y.Services
{
    public class SQLiteService
    {
        private readonly IDbConnection _dbConnection; 

        public SQLiteService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task SaveUserAsync(Users users)
        {
            var query = "INSERT INTO Users (Username) VALUES (@Username)";
            await _dbConnection.ExecuteAsync(query, new
            {
                users.Username, 
            });
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
            var result = await _dbConnection.ExecuteScalarAsync<int>(query, new { Username = username });
            return result > 0;
        }
        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username";
            var user = await _dbConnection.QueryFirstOrDefaultAsync<Users>(query, new { Username = username });
            return user;
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
        public async Task<string> GetSavedArticlesAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);   
            var userId = user.UserId;  
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
            var resultObject = new { Username = username, Articles = articles }; 
            var jsonResult = JsonConvert.SerializeObject(resultObject, Formatting.Indented);
             return jsonResult;
        }

    }
}