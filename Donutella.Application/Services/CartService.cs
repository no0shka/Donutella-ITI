using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Domain.Enums;

namespace Donutella.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Cart?> GetCartAsync(int userId)
    {
        var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userId);
        var cart = carts.FirstOrDefault();
        if (cart is null)
            return null;

        var items = (await _unitOfWork.CartItems
            .FindAsync(ci => ci.CartId == cart.Id && !ci.IsDeleted)).ToList();

        foreach (var item in items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product is not null)
                item.Product = product;
        }

        cart.CartItems = items;
        return cart;
    }

    public async Task AddToCartAsync(int userId, int productId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product is null || product.IsDeleted)
            throw new InvalidOperationException("Product not found.");

        if (product.Stock <= 0)
            throw new InvalidOperationException("This product is out of stock.");

        var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userId);
        var cart = carts.FirstOrDefault();

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        var existingItems = await _unitOfWork.CartItems.FindAsync(
            ci => ci.CartId == cart.Id && ci.ProductId == productId && !ci.IsDeleted);
        var existing = existingItems.FirstOrDefault();

        if (existing is not null)
        {
            if (existing.Quantity + 1 > product.Stock)
                throw new InvalidOperationException(
                    $"Only {product.Stock} items available in stock.");

            existing.Quantity++;
            _unitOfWork.CartItems.Update(existing);
        }
        else
        {
            await _unitOfWork.CartItems.AddAsync(new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = 1,
                IsDeleted = false
            });
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task IncreaseQuantityAsync(int cartItemId)
    {
        var item = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
        if (item is null || item.IsDeleted)
            return;

        var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
        if (product is null || product.IsDeleted)
            return;

        if (item.Quantity + 1 > product.Stock)
            throw new InvalidOperationException(
                $"Only {product.Stock} items available in stock.");

        item.Quantity++;
        _unitOfWork.CartItems.Update(item);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DecreaseQuantityAsync(int cartItemId)
    {
        var item = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
        if (item is null || item.IsDeleted)
            return;

        item.Quantity--;

        // لو وصل ل 0 حذف
        if (item.Quantity <= 0)
        {
            _unitOfWork.CartItems.Delete(item);
        }
        else
        {
            _unitOfWork.CartItems.Update(item);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(int cartItemId)
    {
        var item = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
        if (item is null)
            return;

        _unitOfWork.CartItems.Delete(item);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CheckoutAsync(int userId, string customerName, string address, string phone)
    {
        var cart = await GetCartAsync(userId);

        if (cart is null || !cart.CartItems.Any())
            throw new InvalidOperationException("Cart is empty.");

        decimal total = 0;
        var orderItems = new List<OrderItem>();

        foreach (var cartItem in cart.CartItems)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
            if (product is null || product.IsDeleted)
                continue;

            if (cartItem.Quantity > product.Stock)
                throw new InvalidOperationException(
                    $"Not enough stock for {product.Name}. Available: {product.Stock}.");

            total += product.Price * cartItem.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Price = product.Price
            });
        }

        if (!orderItems.Any())
            throw new InvalidOperationException("Cart is empty.");

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalPrice = total,
            CustomerName = customerName,
            Address = address,
            Phone = phone,
            IsDeleted = false
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        foreach (var orderItem in orderItems)
        {
            orderItem.OrderId = order.Id;
            await _unitOfWork.OrderItems.AddAsync(orderItem);

            // خصم الكمية من الاستوك
            var product = await _unitOfWork.Products.GetByIdAsync(orderItem.ProductId);
            if (product is not null)
            {
                product.Stock -= orderItem.Quantity;
                _unitOfWork.Products.Update(product);
            }
        }

        // فضّي الكارت بعد التشيك اوت
        foreach (var cartItem in cart.CartItems)
        {
            var fresh = await _unitOfWork.CartItems.GetByIdAsync(cartItem.Id);
            if (fresh is not null)
                _unitOfWork.CartItems.Delete(fresh);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
