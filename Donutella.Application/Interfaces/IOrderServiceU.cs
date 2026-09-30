using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Domain.Enums;

namespace Donutella.Application.Interfaces;

public interface IOrderServiceU
{
    Task<IEnumerable<Order>> GetOrdersAsync(int userId);

    Task<Order?> GetOrderByIdAsync(int orderId, int userId);

    Task CancelOrderAsync(int orderId, int userId);

    // ===== Admin =====
    Task<IEnumerable<Order>> GetAllOrdersAsync(string? email = null, OrderStatus? status = null);

    Task<Order?> GetOrderDetailsAsync(int orderId);

    Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
}
