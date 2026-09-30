using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Domain.Enums;

namespace Donutella.Application.Services;

public class OrderServiceU : IOrderServiceU
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderServiceU(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(int userId)
    {
        var orders = (await _unitOfWork.Orders.FindAsync(
            o => o.UserId == userId && !o.IsDeleted))
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        foreach (var order in orders)
        {
            var items = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id);
            order.OrderItems = items.ToList();
        }

        return orders;
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId, int userId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

        if (order is null || order.IsDeleted || order.UserId != userId)
            return null;

        var items = (await _unitOfWork.OrderItems
            .FindAsync(oi => oi.OrderId == order.Id)).ToList();

        foreach (var item in items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product is not null)
                item.Product = product;
        }

        order.OrderItems = items;
        return order;
    }

    public async Task CancelOrderAsync(int orderId, int userId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

        if (order is null || order.IsDeleted || order.UserId != userId)
            throw new InvalidOperationException("Order not found.");

        if (order.Status == OrderStatus.Cancelled)
            return;

        // ممنوع كانسل اوردر اتعمل له Deliver
        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel this order.");

        // رجّع الكميات للاستوك قبل الكانسل
        var items = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id);
        foreach (var item in items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product is not null)
            {
                product.Stock += item.Quantity;
                _unitOfWork.Products.Update(product);
            }
        }

        order.Status = OrderStatus.Cancelled;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
    }

    // ===== Admin: إدارة الأوردرات =====

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? email = null, OrderStatus? status = null)
    {
        var orders = (await _unitOfWork.Orders.FindAsync(o => !o.IsDeleted)).ToList();

        // فلتر بالحالة
        if (status.HasValue)
            orders = orders.Where(o => o.Status == status.Value).ToList();

        // سيرش بإيميل اليوزر
        if (!string.IsNullOrWhiteSpace(email))
        {
            var term = email.Trim();
            var users = await _unitOfWork.Users.FindAsync(u => u.Email.Contains(term));
            var userIds = users.Select(u => u.Id).ToHashSet();
            orders = orders.Where(o => userIds.Contains(o.UserId)).ToList();
        }

        orders = orders.OrderByDescending(o => o.OrderDate).ToList();

        foreach (var order in orders)
        {
            var items = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id);
            order.OrderItems = items.ToList();

            order.User = await _unitOfWork.Users.GetByIdAsync(order.UserId)
                ?? order.User;
        }

        return orders;
    }

    public async Task<Order?> GetOrderDetailsAsync(int orderId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

        if (order is null || order.IsDeleted)
            return null;

        var items = (await _unitOfWork.OrderItems
            .FindAsync(oi => oi.OrderId == order.Id)).ToList();

        foreach (var item in items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product is not null)
                item.Product = product;
        }

        order.OrderItems = items;
        return order;
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new InvalidOperationException("Invalid status.");

        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

        if (order is null || order.IsDeleted)
            throw new InvalidOperationException("Order not found.");

        if (order.Status == status)
            return;

        var items = (await _unitOfWork.OrderItems
            .FindAsync(oi => oi.OrderId == order.Id)).ToList();

        // اتكنسل -> رجّع الاستوك
        if (status == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
        {
            foreach (var item in items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product is not null)
                {
                    product.Stock += item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }
        }
        // رجع من كانسل لحالة تانية -> اخصم الاستوك تاني
        else if (order.Status == OrderStatus.Cancelled && status != OrderStatus.Cancelled)
        {
            foreach (var item in items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product is null || product.IsDeleted)
                    continue;

                if (item.Quantity > product.Stock)
                    throw new InvalidOperationException(
                        $"Not enough stock for {product.Name}. Available: {product.Stock}.");

                product.Stock -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }
        }

        order.Status = status;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
    }
}
