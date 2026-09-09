using Bookstore.Api.Data;
using Bookstore.Api.Middleware;
using Bookstore.Api.Services;
using Bookstore.Shared.Security;
using DbUp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// appsettings.json's connection string deliberately omits the DB credentials — they come from
// the repo-root .env file (local `dotnet run`) or, in Docker, from docker-compose's own
// ConnectionStrings__BookstoreDb override (which already includes them, making this a no-op).
LoadDotEnvIfPresent(Path.Combine(builder.Environment.ContentRootPath, "..", "..", ".env"));

var connectionStringBuilder = new SqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString("BookstoreDb")
    ?? throw new InvalidOperationException("Missing connection string 'BookstoreDb'."));

if (string.IsNullOrEmpty(connectionStringBuilder.UserID))
{
    connectionStringBuilder.UserID = Environment.GetEnvironmentVariable("DB_USER")
        ?? throw new InvalidOperationException("Missing DB_USER environment variable — check .env exists at the repo root (cp .env.example .env).");
    connectionStringBuilder.Password = Environment.GetEnvironmentVariable("SA_PASSWORD")
        ?? throw new InvalidOperationException("Missing SA_PASSWORD environment variable — check .env exists at the repo root (cp .env.example .env).");
}

var connectionString = connectionStringBuilder.ConnectionString;

var identityServerPublicUrl = builder.Configuration["IdentityServer:ValidIssuer"]
    ?? throw new InvalidOperationException("Missing configuration 'IdentityServer:ValidIssuer'.");

// Add services to the container.

builder.Services.AddDbContext<BookstoreDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Bookstore API", Version = "v1" });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            ClientCredentials = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"{identityServerPublicUrl}/connect/token"),
                Scopes = new Dictionary<string, string> { [ApiScopes.Crud] = "Book CRUD operations" }
            },
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{identityServerPublicUrl}/connect/authorize"),
                Scopes = new Dictionary<string, string>
                {
                    [ApiScopes.Search] = "Book search"
                }
            }
        }
    });

    options.OperationFilter<Bookstore.Api.Swagger.AuthorizeCheckOperationFilter>();
});
builder.Services.AddExceptionHandler<BookstoreExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.RequireHttpsMetadata = false;
        // Disable the legacy inbound claim type map (which silently renames claims like "group"
        // to legacy XML/SOAP URIs such as http://schemas.xmlsoap.org/claims/Group) so JWT claim
        // types pass through exactly as issued — required for the SearchAccess policy's literal
        // CustomClaimTypes.Group ("group") check to match.
        options.MapInboundClaims = false;
        options.TokenValidationParameters.ValidIssuer = builder.Configuration["IdentityServer:ValidIssuer"];
        // Deviation from brief: Duende IdentityServer emits an "aud" claim ("bookstore-api") for
        // this ApiResource, but the brief's snippet never configures a ValidAudience, and
        // TokenValidationParameters.ValidateAudience defaults to true — so a real token from
        // IdentityServer fails with "audience 'bookstore-api' is invalid" unless a matching
        // ValidAudience is configured here. Must match the ApiResource name in
        // Bookstore.IdentityServer's Config.ApiResourceDefinitions.
        options.TokenValidationParameters.ValidAudience = "bookstore-api";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.CrudAccess, p => p.RequireClaim("scope", ApiScopes.Crud));
    options.AddPolicy(PolicyNames.SearchAccess, p => p
        .RequireClaim("scope", ApiScopes.Search)
        .RequireClaim(CustomClaimTypes.Group, GroupNames.ProCredit));
});

var app = builder.Build();

ApplyDatabaseMigrations(connectionString);

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookstore API v1");
        options.OAuthAppName("Bookstore Swagger UI");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void LoadDotEnvIfPresent(string path)
{
    if (!File.Exists(path)) return;

    foreach (var line in File.ReadAllLines(path))
    {
        var trimmed = line.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('#')) continue;

        var separatorIndex = trimmed.IndexOf('=');
        if (separatorIndex < 0) continue;

        var key = trimmed[..separatorIndex].Trim();
        var value = trimmed[(separatorIndex + 1)..].Trim();

        // A real environment variable (e.g. set by a CI runner) always wins over .env.
        if (Environment.GetEnvironmentVariable(key) is null)
            Environment.SetEnvironmentVariable(key, value);
    }
}

static void ApplyDatabaseMigrations(string connectionString)
{
    const int maxAttempts = 10;
    const int delaySeconds = 5;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new InvalidOperationException("DbUp migration failed.", result.Error);
            }

            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            Console.WriteLine($"[DbUp] Attempt {attempt}/{maxAttempts} failed: {ex.Message}. Retrying in {delaySeconds}s...");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}
