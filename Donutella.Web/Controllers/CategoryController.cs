using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Web.Models.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace Donutella.Web.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();

        var models = categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });

        return View(models);
    }

    public IActionResult New()
    {
        return View(new CategoryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAdd(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return View("New", model);

        var category = new Category
        {
            Name = model.Name,
            Description = model.Description
        };

        await _categoryService.AddAsync(category);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
            return NotFound();

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
            return NotFound();

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEdit(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Edit", model);

        var category = await _categoryService.GetByIdAsync(model.Id);

        if (category is null)
            return NotFound();

        category.Name = model.Name;
        category.Description = model.Description;

        await _categoryService.UpdateAsync(category);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
            return NotFound();

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _categoryService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}