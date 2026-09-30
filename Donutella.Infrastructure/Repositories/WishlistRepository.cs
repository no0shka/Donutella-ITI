using System;
using System.Collections.Generic;
using System.Text;
using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Donutella.Infrastructure.Repositories;

public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
{
    private readonly AppDbContext _context;

    public WishlistRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId)
    {
        return await _context.Wishlists
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task<Wishlist?> GetByUserAndProductAsync(int userId, int productId)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(w =>
                w.UserId == userId &&
                w.ProductId == productId);
    }
}
