using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;

namespace Donutella.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _reviewRepository.GetAllAsync();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _reviewRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
    {
        return await _reviewRepository.GetByProductIdAsync(productId);
    }

    public async Task AddAsync(Review review)
    {
        await _reviewRepository.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _reviewRepository.Update(review);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review is null)
            return;

        _reviewRepository.Delete(review);
        await _unitOfWork.SaveChangesAsync();
    }
}