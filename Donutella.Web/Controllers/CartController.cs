using Donutella.Application.Interfaces;
using Donutella.Web.Models.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Donutella.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(claim, out var id))
            return id;

        return null;
    }

    private IActionResult RedirectToLogin(string? returnUrl)
        => RedirectToAction("Login", "Account", new { returnUrl });

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Index", "Cart"));

        var cart = await _cartService.GetCartAsync(userId.Value);
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, string? returnUrl = null)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(returnUrl ?? Url.Action("Index", "Cart"));

        try
        {
            await _cartService.AddToCartAsync(userId.Value, productId);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Increase(int id, string? returnUrl = null)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(returnUrl ?? Url.Action("Index", "Cart"));

        try
        {
            await _cartService.IncreaseQuantityAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decrease(int id, string? returnUrl = null)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(returnUrl ?? Url.Action("Index", "Cart"));

        await _cartService.DecreaseQuantityAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id, string? returnUrl = null)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(returnUrl ?? Url.Action("Index", "Cart"));

        await _cartService.RemoveFromCartAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Checkout", "Cart"));

        var cart = await _cartService.GetCartAsync(userId.Value);

        if (cart is null || !cart.CartItems.Any())
        {
            TempData["Error"] = "Cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Cart = cart;
        return View(new CheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Checkout", "Cart"));

        var cart = await _cartService.GetCartAsync(userId.Value);

        if (cart is null || !cart.CartItems.Any())
        {
            TempData["Error"] = "Cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            return View(model);
        }

        try
        {
            await _cartService.CheckoutAsync(
                userId.Value,
                model.CustomerName.Trim(),
                model.Address.Trim(),
                model.Phone.Trim());

            TempData["Success"] = "Order placed successfully.";
            return RedirectToAction("Index", "Order");
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
