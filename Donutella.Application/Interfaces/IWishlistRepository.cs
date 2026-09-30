using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface IWishlistRepository : IGenericRepository<Wishlist>
{
    Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId);

    Task<Wishlist?> GetByUserAndProductAsync(int userId, int productId);
}