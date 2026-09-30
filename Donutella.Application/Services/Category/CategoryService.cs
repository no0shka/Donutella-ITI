using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;

namespace Donutella.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Where(c => !c.IsDeleted);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
            return;

        category.IsDeleted = true;

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync();
    }
}