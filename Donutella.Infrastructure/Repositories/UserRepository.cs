using Donutella.Application.Interfaces;
using Donutella.Domain.Entities;
using Donutella.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Donutella.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Phone == phone && !u.IsDeleted);
    }

    public async Task<User?> GetByResetTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.PasswordResetToken == token && !u.IsDeleted);
    }
}