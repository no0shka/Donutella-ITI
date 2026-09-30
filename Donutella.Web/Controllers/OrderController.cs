using Donutella.Application.Interfaces;
using Donutella.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Donutella.Web.Controllers;

public class OrderController : Controller
{
    private readonly IOrderServiceU _orderService;

    public OrderController(IOrderServiceU orderService)
    {
        _orderService = orderService;
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

    // View my orders
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Index", "Order"));

        var orders = await _orderService.GetOrdersAsync(userId.Value);
        return View(orders);
    }

    // View single order with details
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Details", "Order", new { id }));

        var order = await _orderService.GetOrderByIdAsync(id, userId.Value);

        if (order is null)
            return NotFound();

        return View(order);
    }

    // Cancel order
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Index", "Order"));

        try
        {
            await _orderService.CancelOrderAsync(id, userId.Value);
            TempData["Success"] = "Order cancelled.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    //  Admin orders management 

    // All orders list + email search + status filter
    [Authorize]
    public async Task<IActionResult> Manage(string? email, OrderStatus? status)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Manage", "Order"));

        var orders = await _orderService.GetAllOrdersAsync(email, status);
        ViewBag.Email = email;
        ViewBag.Status = status;
        return View(orders);
    }

    // Edit order status
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Edit", "Order", new { id }));

        var order = await _orderService.GetOrderDetailsAsync(id);

        if (order is null)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, OrderStatus status)
    {
        var userId = GetUserId();
        if (userId is null)
            return RedirectToLogin(Url.Action("Edit", "Order", new { id }));

        if (!Enum.IsDefined(status))
        {
            TempData["Error"] = "Invalid status.";
            return RedirectToAction(nameof(Manage));
        }

        try
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            TempData["Success"] = "Order status updated.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Manage));
    }
}
