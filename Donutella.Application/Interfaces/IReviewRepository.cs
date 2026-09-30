using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
}