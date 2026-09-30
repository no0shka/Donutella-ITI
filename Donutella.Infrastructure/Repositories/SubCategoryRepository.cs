using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Donutella.Infrastructure.Repositories;

public class SubCategoryRepository
    : GenericRepository<SubCategory>, ISubCategoryRepository
{
    private readonly AppDbContext _context;

    public SubCategoryRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SubCategory>> GetAllWithCategoryAsync()
    {
        return await _context.SubCategories
            .Include(sc => sc.Category)
            .ToListAsync();
    }
}