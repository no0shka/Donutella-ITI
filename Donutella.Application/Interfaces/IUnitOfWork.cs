using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }

    IUserRepository Users { get; }

    IGenericRepository<Cart> Carts { get; }

    IGenericRepository<CartItem> CartItems { get; }

    IGenericRepository<Order> Orders { get; }

    IGenericRepository<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync();
}
