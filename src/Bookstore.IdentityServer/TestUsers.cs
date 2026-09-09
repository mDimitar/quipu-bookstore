using System.Security.Claims;
using Bookstore.Shared.Security;
using Duende.IdentityServer.Test;

namespace Bookstore.IdentityServer;

public static class TestUsers
{
    public static List<TestUser> Users =>
        new()
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "Pass123$",
                Claims =
                {
                    new Claim("name", "Alice ProCredit"),
                    new Claim(CustomClaimTypes.Group, GroupNames.ProCredit)
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "Pass123$",
                Claims =
                {
                    new Claim("name", "Bob NoGroup")
                }
            }
        };
}
