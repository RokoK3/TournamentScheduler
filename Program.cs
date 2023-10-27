using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = "https://dev-2awvvkzfc3xx2uia.us.auth0.com/";
    options.ClientId = "OXD1Fba0PAJhRf5opZRzLNC03dphqx10";
    options.ClientSecret = "vh-qXvI-LToYmlNb9u6xB4dJUxDD8D-iN-AB7bthA7XEtNnil0HH3HkAqvTQwwMk";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.CallbackPath = new PathString("/callback");
    options.ClaimsIssuer = "Auth0";
});

builder.WebHost.UseUrls("https://localhost:5202", "http://localhost:5201"); 

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext httpContext) => 
{
    if (!httpContext.User.Identity?.IsAuthenticated ?? true)
    {
        return Results.Redirect("/login");
    }
    return Results.Redirect("/profile");
});

app.MapGet("/login", () => 
{
    return Results.Challenge(new AuthenticationProperties { RedirectUri = "/" }, new[] { "Auth0" });
});


app.MapGet("/logout", async (HttpContext httpContext) =>
{
 
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    var domain = "https://dev-2awvvkzfc3xx2uia.us.auth0.com";
    var postLogoutUri = "https://localhost:5202/";
    var clientId = "OXD1Fba0PAJhRf5opZRzLNC03dphqx10";
    var auth0LogoutUrl = $"{domain}/v2/logout?returnTo={Uri.EscapeDataString(postLogoutUri)}&client_id={clientId}";


    return Results.Redirect(auth0LogoutUrl);
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();
