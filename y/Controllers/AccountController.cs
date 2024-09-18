using Microsoft.AspNetCore.Mvc;
using y.Services;
using System.Threading.Tasks;
using y.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace y.Controllers
{
    public class AccountController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IMemberManager _memberManager;

        public AccountController(IContentService contentService, IMemberManager memberManager)
        {
            _contentService = contentService;
            _memberManager = memberManager;
        }
        [HttpPost]
        public async Task<IActionResult> AddFavoriteArticle(string articleUrl, string title)
        {
            var user = await _memberManager.GetCurrentMemberAsync();
            if (user != null)
            {
                var userId = user.Id;

                // Get the parent "Articles" node (you can use its ID or find it by alias)
                var parentArticlesNode = _contentService.GetById(1184); 

                if (parentArticlesNode != null)
                {
                    // Create a new article under the "Articles" node
                    var newArticle = _contentService.Create("Article", parentArticlesNode, "article");

                    // Set properties
                    newArticle.SetValue("title", title);
                    newArticle.SetValue("uri", articleUrl);
                    newArticle.SetValue("userId", userId);

                    // Save the new article
                    _contentService.SaveAndPublish(newArticle);
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFavoriteArticle(string articleUrl)
        {
            var user = await _memberManager.GetCurrentMemberAsync();
            if (user != null)
            {
                var userId = user.Id.ToString();

                // Get the parent "Articles" node (you can use its ID or find it by alias)
                var parentArticlesNode = _contentService.GetById(1184); 

                if (parentArticlesNode != null)
                {
                    // Find the article by URL and UserId
                    var articles = _contentService.GetPagedChildren(parentArticlesNode.Id, 0, 100, out var totalRecords)
                                    .Where(x => x.ContentType.Alias == "article" &&
                                                x.GetValue<string>("uri") == articleUrl &&
                                                x.GetValue<string>("userId") == userId);

                    var articleToRemove = articles.FirstOrDefault();
                    if (articleToRemove != null)
                    {
                        // Delete the article
                        _contentService.Delete(articleToRemove);
                    }
                }
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<List<Article>> GetSavedArticlesAsync()
        {
            var user = await _memberManager.GetCurrentMemberAsync();
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
                                        Url = x.GetValue<string>("uri"),
                                        Title = x.GetValue<string>("title")
                                    }).ToList();

                    return articles;
                }
            }

            return new List<Article>();
        }

        public async Task<string> GetUserIdAsync()
        {
            var user = await _memberManager.GetCurrentMemberAsync();
            if (user != null)
            {
                var userId = await _memberManager.GetUserIdAsync(user);
                return userId;
            }
            return string.Empty;
        }

    }
}
