using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;

namespace Donutella.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllWithDetailsAsync();
    }
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _productRepository.GetProductWithDetailsAsync(id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int subCategoryId)
    {
        return await _productRepository.GetByCategoryAsync(subCategoryId);
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        return await _productRepository.SearchAsync(searchTerm);
    }

    public async Task AddAsync(Product product)
    {
        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return;

        _productRepository.Delete(product);
        await _unitOfWork.SaveChangesAsync();
    }
}