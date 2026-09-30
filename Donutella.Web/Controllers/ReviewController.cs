using System.Security.Claims;
using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donutella.Web.Controllers;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // =========================
    // ADD REVIEW
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(
        int productId,
        int rating,
        string comment)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        if (rating < 1 || rating > 5)
        {
            TempData["ReviewError"] =
                "Rating must be between 1 and 5.";

            return RedirectToAction(
                "Details",
                "Store",
                new { id = productId });
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            TempData["ReviewError"] =
                "Please write a comment.";

            return RedirectToAction(
                "Details",
                "Store",
                new { id = productId });
        }

        var review = new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = rating,
            Comment = comment.Trim()
        };

        await _reviewService.AddAsync(review);

        TempData["ReviewSuccess"] =
            "Your review has been added successfully.";

        return RedirectToAction(
            "Details",
            "Store",
            new { id = productId });
    }
}