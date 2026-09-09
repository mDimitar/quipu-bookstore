using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookstore.IdentityServer.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty] public string Username { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public string ReturnUrl { get; set; } = "";
    public string? ErrorMessage { get; set; }

    public void OnGet(string returnUrl)
    {
        ReturnUrl = string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = TestUsers.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

        if (user is null)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        var identityServerUser = new Duende.IdentityServer.IdentityServerUser(user.SubjectId)
        {
            DisplayName = user.Username,
            AdditionalClaims = user.Claims
        };

        await HttpContext.SignInAsync(identityServerUser);

        if (!Url.IsLocalUrl(ReturnUrl))
        {
            return Redirect("~/");
        }

        return Redirect(ReturnUrl);
    }
}
