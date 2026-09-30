using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Infrastructure.Data;

namespace Donutella.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context)
        : base(context)
    {
    }
}