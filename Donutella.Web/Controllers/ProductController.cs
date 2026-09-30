using Donutella.Application.Interfaces;
using Donutella.Application.Services;
using Donutella.Domain.Entities;
using Donutella.Web.Models.ViewModels.Product;
using Donutella.Web.Models.ViewModels.Review;
using Donutella.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Donutella.Web.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ISubCategoryService _subCategoryService;
    private readonly IReviewService _reviewService;
    private readonly IWebHostEnvironment _environment;

    public ProductController(
        IProductService productService,
        ISubCategoryService subCategoryService,
        IReviewService reviewService,
        IWebHostEnvironment environment)
    {
        _productService = productService;
        _subCategoryService = subCategoryService;
        _reviewService = reviewService;
        _environment = environment;
    }

    // GET: Product
    public async Task<IActionResult> Index()
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

    // GET: Product/Details/5
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

            Reviews = product.Reviews
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ProductId = r.ProductId,
                    ProductName = product.Name,
                    UserId = r.UserId
                })
                .ToList()
        };

        return View(model);
    }

    // GET: Product/New
    public async Task<IActionResult> New()
    {
        var model = new ProductViewModel();

        await LoadSubCategories(model);

        return View(model);
    }

    // POST: Product/SaveAdd
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAdd(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSubCategories(model);
            return View("New", model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Stock = model.Stock,
            SubCategoryId = model.SubCategoryId
        };

        // Save Main Image
        if (model.MainImageFile is not null)
        {
            product.Image = await SaveImageAsync(model.MainImageFile);
        }

        await _productService.AddAsync(product);

        // Save More Images
        if (model.ImageFiles.Any())
        {
            foreach (var file in model.ImageFiles)
            {
                var imagePath = await SaveImageAsync(file);

                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = imagePath,
                    ProductId = product.Id
                });
            }

            await _productService.UpdateAsync(product);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Product/Edit/5
    public async Task<IActionResult> Edit(int id)
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
                .ToList()
        };

        await LoadSubCategories(model);

        return View(model);
    }

    // POST: Product/SaveEdit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEdit(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSubCategories(model);
            return View("Edit", model);
        }

        var product = await _productService.GetByIdAsync(model.Id);

        if (product is null)
            return NotFound();

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.Stock = model.Stock;
        product.SubCategoryId = model.SubCategoryId;

        // Change Main Image
        if (model.MainImageFile is not null)
        {
            product.Image = await SaveImageAsync(model.MainImageFile);
        }

        // Add New More Images
        if (model.ImageFiles.Any())
        {
            foreach (var file in model.ImageFiles)
            {
                var imagePath = await SaveImageAsync(file);

                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = imagePath,
                    ProductId = product.Id
                });
            }
        }

        await _productService.UpdateAsync(product);

        return RedirectToAction(nameof(Index));
    }

    // GET: Product/Delete/5
    public async Task<IActionResult> Delete(int id)
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
            SubCategoryName = product.SubCategory?.Name ?? string.Empty
        };

        return View(model);
    }

    // POST: Product/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    // POST: Product/AddReview
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(ReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(
                "Details",
                "Store",
                new { id = model.ProductId });
        }

        var review = new Review
        {
            Rating = model.Rating,
            Comment = model.Comment,
            ProductId = model.ProductId,
            UserId = model.UserId
        };

        await _reviewService.AddAsync(review);

        return RedirectToAction(
            "Details",
            "Store",
            new { id = model.ProductId });
    }

    private async Task LoadSubCategories(ProductViewModel model)
    {
        var subCategories = await _subCategoryService.GetAllAsync();

        model.SubCategories = subCategories.Select(sc => new SelectListItem
        {
            Value = sc.Id.ToString(),
            Text = sc.Name
        });
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "products");

        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            fileName);

        using var stream = new FileStream(
            filePath,
            FileMode.Create);

        await file.CopyToAsync(stream);

        return $"/uploads/products/{fileName}";
    }
}