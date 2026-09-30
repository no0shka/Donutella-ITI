using Donutella.Application.Interfaces;
using Donutella.Application.Services;
using Donutella.Web.Models.ViewModels.Product;
using Donutella.Web.Models.ViewModels.Review;
using Microsoft.AspNetCore.Mvc;

namespace Donutella.Web.Controllers;

public class StoreController : Controller
{
    private readonly IProductService _productService;

    public StoreController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Products()
    {
        var products = await _productService.GetAllAsync();

        var models = products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            Image = p.Image,
            SubCategoryId = p.SubCategoryId,
            SubCategoryName = p.SubCategory?.Name ?? string.Empty
        });

        return View(models);
    }
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
            return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Image = product.Image,
            SubCategoryId = product.SubCategoryId,
            SubCategoryName = product.SubCategory?.Name ?? string.Empty,

            ProductImages = product.ProductImages
                .Select(pi => pi.ImageUrl)
                .ToList(),

            AverageRating = product.Reviews.Any()
                ? (decimal)product.Reviews.Average(r => r.Rating)
                : 0m,

            ReviewCount = product.Reviews.Count,

            Reviews = product.Reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                ProductId = r.ProductId,
                ProductName = product.Name,
                UserId = r.UserId
            }).ToList()
        };

        return View(model);
    }
}