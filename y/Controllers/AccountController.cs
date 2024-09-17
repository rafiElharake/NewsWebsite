using Microsoft.AspNetCore.Mvc;
using y.Services;
using System.Threading.Tasks;
using y.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;

namespace y.Controllers
{
    public class AccountController : Controller
    {
        private readonly SQLiteService _sqliteService;
        private readonly IMemberManager _memberManager;

        public AccountController(SQLiteService sqliteService, IMemberManager memberManager)
        {
            _sqliteService = sqliteService;
            _memberManager = memberManager;
        }
        [HttpPost]
        public async Task<IActionResult> AddFavoriteArticle(string articleUrl, string title)
        {
            var user = _memberManager.GetCurrentMemberAsync();
            if (user != null)
            {
                await _sqliteService.AddFavoriteArticleAsync(Int32.Parse(user.Result.Id), articleUrl, title);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFavoriteArticle(string articleUrl)
        {
            await _sqliteService.RemoveFavoriteArticleAsync(articleUrl);
            return Redirect(Request.Headers["Referer"].ToString());
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
