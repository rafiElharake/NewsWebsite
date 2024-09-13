using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace y
{
    public class SearchQueryController : RenderController
    {
        public SearchQueryController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }
        [Route("")]
        public override IActionResult Index()
        {
            var query = Request.Query["query"].FirstOrDefault();
            ViewBag.Query = query;
            var page = Request.Query["page"].FirstOrDefault();
            ViewBag.Page = page;
            return CurrentTemplate(CurrentPage);
        }
    }
}