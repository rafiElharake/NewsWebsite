using y.Models;
using y.Services;
namespace y.Services
{
    public class NewsServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly SQLiteService _sqliteService;

        public NewsServices(IConfiguration configuration, SQLiteService sqliteService)
        {
            _httpClient = new HttpClient();
            _configuration = configuration; 
            _sqliteService = sqliteService;
        } 
        public async Task<string> GetTopHeadlinesAsync(string query, string username)
        {
            if (query != "saved") {
            var apiKey = _configuration["ApiSettings:NewsApiKey"];
            System.Diagnostics.Debug.WriteLine("b");
            if (query == null) { query = "tech"; }
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API key is missing or empty.");
            } 
            var requestUri = $"https://gnews.io/api/v4/search?q={query}&lang=en&country=us&max=10&apikey={apiKey}";
           try
            {
                var response = await _httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error fetching top headlines: {response.StatusCode} - {responseContent}");
                } 
                var responseString = await response.Content.ReadAsStringAsync();
                
                return responseString;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            }
            else
            { 
               var responseString = await _sqliteService.GetSavedArticlesAsync(username);
                return responseString;
            } 
        }
    }
}
