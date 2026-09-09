using Bookstore.Shared.Security;
using Duende.IdentityServer.Models;

namespace Bookstore.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResourceDefinitions =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopeDefinitions =>
        new ApiScope[]
        {
            new(ApiScopes.Crud, "Book CRUD operations"),
            new(ApiScopes.Search, "Book search") { UserClaims = { CustomClaimTypes.Group } }
        };

    public static IEnumerable<ApiResource> ApiResourceDefinitions =>
        new ApiResource[]
        {
            new("bookstore-api", "Bookstore API")
            {
                Scopes = { ApiScopes.Crud, ApiScopes.Search }
            }
        };

    public static IEnumerable<Client> GetClientDefinitions(string searchClientRedirectUri, string swaggerOrigin) =>
        new Client[]
        {
            new()
            {
                ClientId = "bookstore.crud.client",
                ClientSecrets = { new Secret("dev-secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { ApiScopes.Crud },
                AllowedCorsOrigins = { swaggerOrigin }
            },
            new()
            {
                ClientId = "bookstore.search.client",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = { searchClientRedirectUri },
                AllowedScopes = { "openid", "profile", ApiScopes.Search },
                RequireConsent = false,
                AllowedCorsOrigins = { swaggerOrigin }
            }
        };
}
