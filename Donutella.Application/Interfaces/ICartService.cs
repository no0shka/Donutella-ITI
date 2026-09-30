using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface ICartService
{
    Task<Cart?> GetCartAsync(int userId);

    Task AddToCartAsync(int userId, int productId);

    Task IncreaseQuantityAsync(int cartItemId);

    Task DecreaseQuantityAsync(int cartItemId);

    Task RemoveFromCartAsync(int cartItemId);

    Task CheckoutAsync(int userId, string customerName, string address, string phone);
}