using Donutella.Domain.Entities;

namespace Donutella.Application.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<IEnumerable<Product>> GetByCategoryAsync(int subCategoryId);

    Task<IEnumerable<Product>> SearchAsync(string searchTerm);

    Task AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task DeleteAsync(int id);
}