using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;

namespace Donutella.Application.Services;

public class SubCategoryService : ISubCategoryService
{
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubCategoryService(
        ISubCategoryRepository subCategoryRepository,
        IUnitOfWork unitOfWork)
    {
        _subCategoryRepository = subCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SubCategory>> GetAllAsync()
    {
        return await _subCategoryRepository.GetAllWithCategoryAsync();
    }

    public async Task<SubCategory?> GetByIdAsync(int id)
    {
        return await _subCategoryRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(SubCategory subCategory)
    {
        await _subCategoryRepository.AddAsync(subCategory);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(SubCategory subCategory)
    {
        _subCategoryRepository.Update(subCategory);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var subCategory = await _subCategoryRepository.GetByIdAsync(id);

        if (subCategory is null)
            return;

        _subCategoryRepository.Delete(subCategory);
        await _unitOfWork.SaveChangesAsync();
    }
}