using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Donutella.Infrastructure.Repositories;

public class ProductRepository
    : GenericRepository<Product>, IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
    {
        return await _context.Products
            .Include(p => p.SubCategory)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int subCategoryId)
    {
        return await _context.Products
            .Include(p => p.SubCategory)
            .Where(p =>
                p.SubCategoryId == subCategoryId &&
                !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        return await _context.Products
            .Include(p => p.SubCategory)
            .Where(p =>
                !p.IsDeleted &&
                (p.Name.Contains(searchTerm) ||
                 p.Description.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsAsync(int id)
    {
        return await _context.Products
            .Include(p => p.SubCategory)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                !p.IsDeleted);
    }
}