using System.Data;
using y.Services; 
using Microsoft.EntityFrameworkCore;
using y;
using Microsoft.AspNetCore.Identity;
using Umbraco.Cms.Core.Security;
 

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();
 
builder.Services.AddHttpClient();
builder.Services.AddScoped<NewsServices>();


var app = builder.Build();

app.UseSession(); 
await app.BootUmbracoAsync(); 
app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });
app.MapControllerRoute(
    name: "customRoute",
    pattern: "{controller=SearchQuery}/{action=Search}"
);

// Run the application
await app.RunAsync();
