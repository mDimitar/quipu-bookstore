var builder = WebApplication.CreateBuilder(args);

var searchClientRedirectUri = builder.Configuration["Clients:SearchRedirectUri"]
    ?? throw new InvalidOperationException("Missing configuration 'Clients:SearchRedirectUri'.");
var swaggerOrigin = new Uri(searchClientRedirectUri).GetLeftPart(UriPartial.Authority);

builder.Services.AddRazorPages();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.OnAppendCookie = cookieContext => DowngradeSameSiteIfNotHttps(cookieContext.Context, cookieContext.CookieOptions);
    options.OnDeleteCookie = cookieContext => DowngradeSameSiteIfNotHttps(cookieContext.Context, cookieContext.CookieOptions);
});

builder.Services.AddIdentityServer(options =>
    {
        builder.Configuration.GetSection("IdentityServer").Bind(options);
    })
    .AddInMemoryIdentityResources(Bookstore.IdentityServer.Config.IdentityResourceDefinitions)
    .AddInMemoryApiScopes(Bookstore.IdentityServer.Config.ApiScopeDefinitions)
    .AddInMemoryApiResources(Bookstore.IdentityServer.Config.ApiResourceDefinitions)
    .AddInMemoryClients(Bookstore.IdentityServer.Config.GetClientDefinitions(searchClientRedirectUri, swaggerOrigin))
    .AddTestUsers(Bookstore.IdentityServer.TestUsers.Users);

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseIdentityServer();
app.MapRazorPages();

app.Run();

static void DowngradeSameSiteIfNotHttps(HttpContext httpContext, CookieOptions options)
{
    if (options.SameSite == SameSiteMode.None && !httpContext.Request.IsHttps)
    {
        options.SameSite = SameSiteMode.Unspecified;
    }
}
