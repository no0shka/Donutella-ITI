using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetAllAsync();

    Task<Review?> GetByIdAsync(int id);

    Task<IEnumerable<Review>> GetByProductIdAsync(int productId);

    Task AddAsync(Review review);

    Task UpdateAsync(Review review);

    Task DeleteAsync(int id);
}