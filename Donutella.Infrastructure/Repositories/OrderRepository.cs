using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Domain.Enums;
using Donutella.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donutella.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(o => o.UserId == userId && !o.IsDeleted)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Where(o => o.Id == id && !o.IsDeleted)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.SubCategory)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<OrderStatus>(status, out var orderStatus))
            return Enumerable.Empty<Order>();

        return await _dbSet
            .Where(o => o.Status == orderStatus && !o.IsDeleted)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _dbSet
            .Where(o => !o.IsDeleted)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
            query = query.Where(o => o.Status == orderStatus);

        if (fromDate.HasValue)
            query = query.Where(o => o.OrderDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(o => o.OrderDate <= toDate.Value);

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _dbSet.Where(o => !o.IsDeleted && o.Status == OrderStatus.Delivered).AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(o => o.OrderDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(o => o.OrderDate <= toDate.Value);

        return await query.SumAsync(o => o.TotalPrice);
    }

    public async Task<int> GetOrdersCountAsync(string? status = null)
    {
        var query = _dbSet.Where(o => !o.IsDeleted).AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
            query = query.Where(o => o.Status == orderStatus);

        return await query.CountAsync();
    }
}