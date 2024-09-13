using y.Services;

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

// Register services
builder.Services.AddHttpClient();
builder.Services.AddScoped<NewsServices>();
builder.Services.AddRazorPages();
 
var app = builder.Build();

// Boot Umbraco and use its middleware
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
