using Donutella.Application.Interfaces;
using Donutella.Infrastructure.Data;
using Donutella.Domain.Entities;

namespace Donutella.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IProductRepository? _products;
    private IUserRepository? _users;

    #region omnia

    private IGenericRepository<Cart>? _carts;
    private IGenericRepository<CartItem>? _cartItems;
    private IGenericRepository<Order>? _orders;
    private IGenericRepository<OrderItem>? _orderItems;

    public IGenericRepository<Cart> Carts =>
        _carts ??= new GenericRepository<Cart>(_context);

    public IGenericRepository<CartItem> CartItems =>
        _cartItems ??= new GenericRepository<CartItem>(_context);

    public IGenericRepository<Order> Orders =>
        _orders ??= new GenericRepository<Order>(_context);

    public IGenericRepository<OrderItem> OrderItems =>
        _orderItems ??= new GenericRepository<OrderItem>(_context);

    #endregion

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products =>
        _products ??= new ProductRepository(_context);

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
