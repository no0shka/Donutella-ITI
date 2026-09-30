using System;
using System.Collections.Generic;
using System.Text;
using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;

namespace Donutella.Application.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IUnitOfWork unitOfWork)
    {
        _wishlistRepository = wishlistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId)
    {
        return await _wishlistRepository.GetByUserIdAsync(userId);
    }

    public async Task AddAsync(int userId, int productId)
    {
        var exists = await _wishlistRepository
            .GetByUserAndProductAsync(userId, productId);

        if (exists is not null)
            return;

        var wishlist = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };

        await _wishlistRepository.AddAsync(wishlist);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveAsync(int userId, int productId)
    {
        var wishlist = await _wishlistRepository
            .GetByUserAndProductAsync(userId, productId);

        if (wishlist is null)
            return;

        _wishlistRepository.Delete(wishlist);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int userId, int productId)
    {
        var wishlist = await _wishlistRepository
            .GetByUserAndProductAsync(userId, productId);

        return wishlist is not null;
    }
}
