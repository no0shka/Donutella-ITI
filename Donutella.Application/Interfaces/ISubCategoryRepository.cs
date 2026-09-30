using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface ISubCategoryRepository : IGenericRepository<SubCategory>
{
    Task<IEnumerable<SubCategory>> GetAllWithCategoryAsync();
}