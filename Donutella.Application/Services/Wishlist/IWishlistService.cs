using System;
using System.Collections.Generic;
using System.Text;
using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId);

    Task AddAsync(int userId, int productId);

    Task RemoveAsync(int userId, int productId);

    Task<bool> ExistsAsync(int userId, int productId);
}
