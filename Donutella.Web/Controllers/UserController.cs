using Donutella.Application.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donutella.Web.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = await _userService.GetByIdAsync(userId);
        
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = await _userService.GetByIdAsync(userId);
        
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(string name, string phone)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _userService.UpdateProfileAsync(userId, name, phone);
        
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            var user = await _userService.GetByIdAsync(userId);
            return View(user);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "كلمة المرور الجديدة غير متطابقة");
            return View();
        }

        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);
        
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            return View();
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Profile));
    }
}