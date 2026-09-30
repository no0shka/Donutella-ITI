using Donutella.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donutella.Application.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<Order?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetByStatusAsync(string status);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<int> GetOrdersCountAsync(string? status = null);
}