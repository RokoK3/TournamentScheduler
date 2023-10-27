using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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

    // Set the callback path, so Auth0 will call back to http://localhost:5001/signin-auth0 
    // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
    options.CallbackPath = new Microsoft.AspNetCore.Http.PathString("/signin-auth0");

    // Configure the Claims Issuer to be Auth0
    options.ClaimsIssuer = "Auth0";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use authentication and authorization:
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();