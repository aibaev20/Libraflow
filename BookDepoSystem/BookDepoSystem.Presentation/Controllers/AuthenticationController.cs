using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookDepoSystem.Common;
using BookDepoSystem.Data;
using BookDepoSystem.Presentation.Extensions;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Identity.Extensions;
using Essentials.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookDepoSystem.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AuthenticationController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IEmailService emailService;
    private readonly UrlEncoder urlEncoder;
    private readonly ILogger<AuthenticationController> logger;
    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        UrlEncoder urlEncoder,
        ILogger<AuthenticationController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.emailService = emailService;
        this.urlEncoder = urlEncoder;
        this.logger = logger;
    }

    [HttpGet("/login")]
    // the login functions on /login
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new LoginViewModel();
        return this.View(model);
        // make sure that we have the model in the view
    }

    // overload on the login
    [HttpPost("/login")]
    [ValidateAntiForgeryToken] // using this to not allow intercepting
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user == null || !(await this.userManager.CheckPasswordAsync(user, model.Password!)))
            {
                this.ModelState.AddModelError(string.Empty, Common.T.InvalidLoginErrorMessage);
                return this.View(model);
            }

            if (await this.userManager.IsLockedOutAsync(user))
            {
                this.ModelState.AddModelError(string.Empty, Common.T.UserLockedOutErrorMessage);
                return this.View(model);
            }

            await this.SignInAsync(user, model.RememberMe);
            return this.RedirectToDefault();
        }

        return this.View(model);
        // this way we can submit the form
    }

    /*
    [HttpGet("/register")]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new RegisterViewModel();
        return this.View(model);
    }
    [HttpPost("/register")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var result = await this.userManager.CreateAsync(
                new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                },
                model.Password!);
            if (result.Succeeded)
            {
                this.TempData["MessageText"] = T.RegisterSuccessMessage;
                this.TempData["MessageVariant"] = "success";
                return this.RedirectToAction(nameof(this.Login));
            }
            this.ModelState.AssignIdentityErrors(result.Errors);
        }
        return this.View(model);
    }
    */

    [HttpGet("/forgot-password")]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        var model = new ForgotPasswordViewModel();
        return this.View(model);
    }

    [HttpPost("/forgot-password")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user != null)
            {
                var resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordEncodedToken = UrlEncoder.Default.Encode(resetPasswordToken);
                var resetPasswordUrl = this.HttpContext
                    .GetAbsoluteRoute($"/reset-password?email={user.Email}&token={resetPasswordEncodedToken}");
                var result = await this.emailService.SendResetPasswordEmailAsync(
                    user.Email!,
                    resetPasswordUrl);

                this.logger.LogInformation("Reset password email send result {Result}", result);
            }

            this.TempData["MessageText"] = T.ForgotPasswordSuccessMessage;
            this.TempData["MessageVariant"] = "success";
            return this.RedirectToAction(nameof(this.Login));
        }

        return this.View(model);
    }

    [AllowAnonymous]
    [HttpGet("/reset-password")]
    public async Task<IActionResult> ResetPassword(string email, string token)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return this.NotFound();
        }

        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return this.NotFound();
        }

        var model = new ResetPasswordViewModel
        {
            Token = token,
            Email = email,
        };

        return this.View(model);
    }

    [HttpPost("/reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (this.IsUserAuthenticated())
        {
            return this.RedirectToDefault();
        }

        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                return this.NotFound();
            }

            var result = await this.userManager.ResetPasswordAsync(user, model.Token!, model.Password!);
            if (result.Succeeded)
            {
                return this.RedirectToAction(nameof(this.Login));
            }

            this.ModelState.AssignIdentityErrors(result.Errors);
        }

        return this.View(model);
    }

    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return this.RedirectToAction(nameof(this.Login));
    }

    private async Task SignInAsync(
        ApplicationUser user,
        bool rememberMe)
    {
        var claimsPrinciple = await this.signInManager
            .ClaimsFactory
            .CreateAsync(user);

        await this.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrinciple,
            new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                IsPersistent = rememberMe,
            });
    }
}