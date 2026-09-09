var builder = WebApplication.CreateBuilder(args);

var searchClientRedirectUri = builder.Configuration["Clients:SearchRedirectUri"]
    ?? throw new InvalidOperationException("Missing configuration 'Clients:SearchRedirectUri'.");

builder.Services.AddRazorPages();

builder.Services.AddIdentityServer(options =>
    {
        builder.Configuration.GetSection("IdentityServer").Bind(options);
    })
    .AddInMemoryIdentityResources(Bookstore.IdentityServer.Config.IdentityResourceDefinitions)
    .AddInMemoryApiScopes(Bookstore.IdentityServer.Config.ApiScopeDefinitions)
    .AddInMemoryApiResources(Bookstore.IdentityServer.Config.ApiResourceDefinitions)
    .AddInMemoryClients(Bookstore.IdentityServer.Config.GetClientDefinitions(searchClientRedirectUri))
    .AddTestUsers(Bookstore.IdentityServer.TestUsers.Users);

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.MapRazorPages();

app.Run();
