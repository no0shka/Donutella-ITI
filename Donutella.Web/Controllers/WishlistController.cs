using System.Security.Claims;
using Donutella.Application.Interfaces;
using Donutella.Web.Models.ViewModels.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donutella.Web.Controllers;

public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    // ==============================
    // Wishlist Page
    // ==============================
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var wishlist = await _wishlistService.GetByUserIdAsync(userId.Value);

        var viewModel = wishlist.Select(w => new WishlistViewModel
        {
            Id = w.Id,
            UserId = w.UserId,
            ProductId = w.ProductId,
            ProductName = w.Product.Name,
            ProductPrice = w.Product.Price,
            ProductImage = w.Product.Image
        }).ToList();

        return View(viewModel);
    }


    // ==============================
    // Add To Wishlist
    // ==============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [HttpPost]
    
    public async Task<IActionResult> Add(
    int productId,
    string? returnUrl = null)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return RedirectToAction(
                "Login",
                "Account",
                new
                {
                    returnUrl = returnUrl ??
                        Url.Action(
                            "Details",
                            "Product",
                            new { id = productId })
                });
        }

        await _wishlistService.AddAsync(
            userId.Value,
            productId);

        return RedirectToAction("Index");
    }


    // ==============================
    // Remove From Wishlist
    // ==============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Remove(
        int productId,
        string? returnUrl = null)
    {
        var userId = GetUserId();

        if (userId == null)
            return RedirectToAction("Login", "Account");

        await _wishlistService.RemoveAsync(
            userId.Value,
            productId);

        if (!string.IsNullOrEmpty(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index");
    }


    // ==============================
    // Check Wishlist
    // ==============================
    [HttpGet]
    public async Task<IActionResult> Exists(int productId)
    {
        var userId = GetUserId();

        if (userId == null)
            return Json(new { exists = false });

        var exists = await _wishlistService.ExistsAsync(
            userId.Value,
            productId);

        return Json(new { exists });
    }


    // ==============================
    // Get Current User ID
    // ==============================
    private int? GetUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdClaim, out int userId))
            return userId;

        return null;
    }
}