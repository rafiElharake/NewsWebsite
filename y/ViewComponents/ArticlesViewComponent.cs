using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using y.Models;
using y.Services;

public class ArticlesViewComponent : ViewComponent
{
    private readonly NewsServices _newsService; 
    
    public ArticlesViewComponent(NewsServices newsService)
    {
        _newsService = newsService;
    } 
    public async Task<IViewComponentResult> InvokeAsync(string query, int page, MemberIdentityUser user, IContentService _contentService)
    {
        NewsResponse newsModel = null;
        List<Article> current = new List<Article>(); 
        try
        {
            var newsResponse = await _newsService.GetTopHeadlinesAsync(query, user, _contentService);
            newsModel = JsonConvert.DeserializeObject<NewsResponse>(newsResponse);

            if (newsModel != null && newsModel.Articles != null)
            {
                for (int i = 2 * (page - 1); i < 2 * page; i++)
                {
                    if (i >= 0 && i < newsModel.Articles.Count)
                    {
                        current.Add(newsModel.Articles[i]);
                    }
                }
            }
        }
        catch (Exception ex)
        { 
        }
        ViewData["TotalArticles"] = newsModel?.Articles?.Count ?? 0;
        return View(current);
    }
}
