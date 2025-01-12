using System.Collections.Generic;
using BookDepoSystem.Data;
using BookDepoSystem.Presentation;
using BookDepoSystem.Services;
using BookDepoSystem.Services.Identity.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // default scheme
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme, // cookie configurations
        options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
            options.AccessDeniedPath = "/access-denied";
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(DefaultPolicies.AdminPolicy, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
        policyBuilder.RequireRole(DefaultRoles.Admin);
    });
    options.AddPolicy(DefaultPolicies.UserPolicy, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
        policyBuilder.RequireRole(DefaultRoles.User);
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddData(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

builder.Services.AddMvc();

var app = builder.Build();

await app.PrepareAsync();

if (app.Environment.IsDevelopment())
{
    // do nothing
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();