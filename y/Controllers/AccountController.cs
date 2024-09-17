using Microsoft.AspNetCore.Mvc;
using y.Services;
using System.Threading.Tasks;
using y.Models;
using Umbraco.Cms.Core.Models.Membership;

namespace y.Controllers
{
    public class AccountController : Controller
    {
        private readonly SQLiteService _sqliteService;

        public AccountController(SQLiteService sqliteService)
        {
            _sqliteService = sqliteService;
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return Redirect("/");
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(string username)
        {
            if (await _sqliteService.UsernameExistsAsync(username))
            {
                HttpContext.Session.SetString("Username", username);
                return Redirect("/");
            }
            await _sqliteService.SaveUserAsync(new Users { Username = username });
            HttpContext.Session.SetString("Username", username);
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteArticle(string articleUrl, string title)
        {
            var username = HttpContext.Session.GetString("Username"); 
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(); 
            } 
            var user = await _sqliteService.GetUserByUsernameAsync(username);
            if (user != null)
            {
                await _sqliteService.AddFavoriteArticleAsync(user.UserId, articleUrl, title);
            }

            return Redirect("/");
        }
        public string userid() { 
        return HttpContext.Session.GetString("Username");
        }
    }
}
