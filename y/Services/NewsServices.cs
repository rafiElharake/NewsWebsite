using Newtonsoft.Json;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;
using y.Models;
using y.Services;
namespace y.Services
{
    public class NewsServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public NewsServices(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }
        public async Task<string> GetTopHeadlinesAsync(string query, MemberIdentityUser user, IContentService _contentService)
        {
            if (query != "saved")
            {
                var siteConfigNode = _contentService.GetRootContent().FirstOrDefault(x => x.ContentType.Alias == "siteConfiguration"); 
                var apiKey = siteConfigNode?.GetValue<string>("ApiKey");
                if (query == null) { query = "tech"; }
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("API key is missing or empty.");
                }
                var requestUri = $"https://gnews.io/api/v4/search?q={query}&lang=en&country=us&max=15&apikey={apiKey}";
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
                catch (HttpRequestException)
                {
                    throw;
                }
            }
            else
            {
                if (user != null)
                {
                    var userId = user.Id.ToString();
                    var parentArticlesNode = _contentService.GetById(1184);
                    if (parentArticlesNode != null)
                    {
                        var articles = _contentService.GetPagedChildren(parentArticlesNode.Id, 0, 100, out var totalRecords)
                                        .Where(x => x.ContentType.Alias == "article" &&
                                                    x.GetValue<string>("userId") == userId)
                                        .Select(x => new Article
                                        {
                                            Url = x.GetValue<string>("uri") ?? string.Empty,
                                            Title = x.GetValue<string>("title") ?? string.Empty
                                        }).ToList();
                        var resultObject = new { Username = user.UserName, Articles = articles };
                        return JsonConvert.SerializeObject(resultObject, Formatting.Indented);
                    }
                }

                return "";

            }
        }
    }
}