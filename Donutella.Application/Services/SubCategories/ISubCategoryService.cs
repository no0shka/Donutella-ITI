using System;
using System.Collections.Generic;
using System.Text;
using Donutella.Domain.Entities;

namespace Donutella.Application.Interfaces;

public interface ISubCategoryService
{
    Task<IEnumerable<SubCategory>> GetAllAsync();

    Task<SubCategory?> GetByIdAsync(int id);

    Task AddAsync(SubCategory subCategory);

    Task UpdateAsync(SubCategory subCategory);

    Task DeleteAsync(int id);
}
