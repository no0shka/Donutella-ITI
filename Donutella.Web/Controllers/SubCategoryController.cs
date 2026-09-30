using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Web.Models.ViewModels.SubCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Donutella.Web.Controllers;

public class SubCategoryController : Controller
{
    private readonly ISubCategoryService _subCategoryService;
    private readonly ICategoryService _categoryService;

    public SubCategoryController(
        ISubCategoryService subCategoryService,
        ICategoryService categoryService)
    {
        _subCategoryService = subCategoryService;
        _categoryService = categoryService;
    }

    // GET: SubCategory
    public async Task<IActionResult> Index()
    {
        var subCategories = await _subCategoryService.GetAllAsync();

        var models = subCategories.Select(sc => new SubCategoryViewModel
        {
            Id = sc.Id,
            Name = sc.Name,
            Description = sc.Description,
            CategoryId = sc.CategoryId,
            CategoryName = sc.Category?.Name ?? string.Empty
        });

        return View(models);
    }

    // GET: SubCategory/New
    public async Task<IActionResult> New()
    {
        var model = new SubCategoryViewModel();

        await LoadCategories(model);

        return View(model);
    }

    // POST: SubCategory/SaveAdd
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAdd(SubCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories(model);
            return View("New", model);
        }

        var subCategory = new SubCategory
        {
            Name = model.Name,
            Description = model.Description,
            CategoryId = model.CategoryId
        };

        await _subCategoryService.AddAsync(subCategory);

        return RedirectToAction(nameof(Index));
    }

    // GET: SubCategory/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var subCategory = await _subCategoryService.GetByIdAsync(id);

        if (subCategory is null)
            return NotFound();

        var model = new SubCategoryViewModel
        {
            Id = subCategory.Id,
            Name = subCategory.Name,
            Description = subCategory.Description,
            CategoryId = subCategory.CategoryId,
            CategoryName = subCategory.Category?.Name ?? string.Empty
        };

        return View(model);
    }

    // GET: SubCategory/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var subCategory = await _subCategoryService.GetByIdAsync(id);

        if (subCategory is null)
            return NotFound();

        var model = new SubCategoryViewModel
        {
            Id = subCategory.Id,
            Name = subCategory.Name,
            Description = subCategory.Description,
            CategoryId = subCategory.CategoryId,
            CategoryName = subCategory.Category?.Name ?? string.Empty
        };

        await LoadCategories(model);

        return View(model);
    }

    // POST: SubCategory/SaveEdit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEdit(SubCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories(model);
            return View("Edit", model);
        }

        var subCategory = await _subCategoryService.GetByIdAsync(model.Id);

        if (subCategory is null)
            return NotFound();

        subCategory.Name = model.Name;
        subCategory.Description = model.Description;
        subCategory.CategoryId = model.CategoryId;

        await _subCategoryService.UpdateAsync(subCategory);

        return RedirectToAction(nameof(Index));
    }

    // GET: SubCategory/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var subCategory = await _subCategoryService.GetByIdAsync(id);

        if (subCategory is null)
            return NotFound();

        var model = new SubCategoryViewModel
        {
            Id = subCategory.Id,
            Name = subCategory.Name,
            Description = subCategory.Description,
            CategoryId = subCategory.CategoryId,
            CategoryName = subCategory.Category?.Name ?? string.Empty
        };

        return View(model);
    }

    // POST: SubCategory/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _subCategoryService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategories(SubCategoryViewModel model)
    {
        var categories = await _categoryService.GetAllAsync();

        model.Categories = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        });
    }
}