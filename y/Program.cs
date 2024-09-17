using System.Data;
using y.Services;
using System.Data.SQLite;
using Microsoft.Data.Sqlite; 
using Microsoft.EntityFrameworkCore;
using y;
using Microsoft.AspNetCore.Identity;  
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


var sqliteConnection = new SqliteConnection("Data Source=Umbraco.sqlite.db");
builder.Services.AddSingleton(new SQLiteService(sqliteConnection));

builder.Services.AddScoped<IDbConnection>(sp =>
{
    return new SqliteConnection("Data Source=Umbraco.sqlite.db");
});

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
