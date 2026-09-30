using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Donutella.Infrastructure.Repositories;

public class ReviewRepository
    : GenericRepository<Review>, IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId)
            .ToListAsync();
    }
}