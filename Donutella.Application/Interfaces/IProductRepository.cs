using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithDetailsAsync();

    Task<IEnumerable<Product>> GetByCategoryAsync(int subCategoryId);

    Task<IEnumerable<Product>> SearchAsync(string searchTerm);

    Task<Product?> GetProductWithDetailsAsync(int id);
}