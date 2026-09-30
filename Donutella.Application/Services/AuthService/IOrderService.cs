using Donutella.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donutella.Application.Services.AuthService;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<(bool Success, string Message, Order? Order)> CreateOrderAsync(int userId, int[] cartItemIds);
    Task<(bool Success, string Message)> UpdateStatusAsync(int orderId, string status);
    Task<(bool Success, string Message)> CancelOrderAsync(int orderId, int userId, bool isAdmin = false);
    Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<int> GetOrdersCountAsync(string? status = null);
    Task<int> GetPendingOrdersCountAsync();
    Task<Dictionary<string, int>> GetOrdersByStatusAsync();
    Task<Dictionary<DateTime, decimal>> GetRevenueLastDaysAsync(int days);
}