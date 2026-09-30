using Donutella.Application.Services.AuthService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Donutella.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    // =========================
    // LOGIN - GET
    // =========================
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    // =========================
    // LOGIN - POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email,
        string password,
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                "",
                "البريد الإلكتروني وكلمة المرور مطلوبان");

            return View();
        }

        var result = await _authService.LoginAsync(
            email,
            password);

        if (!result.Success || result.User == null)
        {
            ModelState.AddModelError(
                "",
                result.Message);

            return View();
        }

        // =========================
        // Claims
        // =========================
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                result.User.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                result.User.Email),

            new Claim(
                ClaimTypes.Role,
                result.User.Role.ToString()),

            new Claim(
                "Token",
                result.Token)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // =========================
        // Return URL
        // =========================
        if (!string.IsNullOrEmpty(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home");
    }

    // =========================
    // REGISTER - GET
    // =========================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // =========================
    // REGISTER - POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string name,
        string email,
        string password,
        string confirmPassword,
        string phone = "")
    {
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            ModelState.AddModelError(
                "",
                "All required fields must be filled.");

            return View();
        }

        if (password != confirmPassword)
        {
            ModelState.AddModelError(
                "",
                "Passwords do not match.");

            return View();
        }

        if (password.Length < 6)
        {
            ModelState.AddModelError(
                "",
                "Password must be at least 6 characters.");

            return View();
        }

        var result = await _authService.RegisterAsync(
            name,
            email,
            password,
            phone);

        if (!result.Success || result.User == null)
        {
            ModelState.AddModelError(
                "",
                result.Message);

            return View();
        }

        // =========================
        // Claims
        // =========================
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                result.User.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                result.User.Email),

            new Claim(
                ClaimTypes.Role,
                result.User.Role.ToString()),

            new Claim(
                "Token",
                result.Token)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction(
            "Index",
            "Home");
    }

    // =========================
    // FORGOT PASSWORD - GET
    // =========================
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // =========================
    // FORGOT PASSWORD - POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(
                "",
                "البريد الإلكتروني مطلوب");

            return View();
        }

        var result =
            await _authService.ForgotPasswordAsync(email);

        TempData["Message"] = result.Message;
        TempData["Success"] = result.Success;

        return RedirectToAction(
            nameof(ForgotPasswordConfirmation));
    }

    // =========================
    // FORGOT PASSWORD CONFIRMATION
    // =========================
    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        ViewBag.Message = TempData["Message"];
        ViewBag.Success = TempData["Success"];

        return View();
    }

    // =========================
    // RESET PASSWORD - GET
    // =========================
    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction(
                nameof(ForgotPassword));
        }

        ViewBag.Token = token;

        return View();
    }

    // =========================
    // RESET PASSWORD - POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        string token,
        string newPassword,
        string confirmPassword)
    {
        if (string.IsNullOrEmpty(token))
        {
            ModelState.AddModelError(
                "",
                "رمز إعادة التعيين غير صالح");

            ViewBag.Token = token;

            return View();
        }

        if (string.IsNullOrEmpty(newPassword) ||
            string.IsNullOrEmpty(confirmPassword))
        {
            ModelState.AddModelError(
                "",
                "كلمة المرور وتأكيدها مطلوبان");

            ViewBag.Token = token;

            return View();
        }

        var result =
            await _authService.ResetPasswordAsync(
                token,
                newPassword,
                confirmPassword);

        if (!result.Success)
        {
            ModelState.AddModelError(
                "",
                result.Message);

            ViewBag.Token = token;

            return View();
        }

        TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(
            nameof(Login));
    }

    // =========================
    // LOGOUT
    // =========================
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(
            nameof(Register));
    }

    // =========================
    // ACCESS DENIED
    // =========================
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}