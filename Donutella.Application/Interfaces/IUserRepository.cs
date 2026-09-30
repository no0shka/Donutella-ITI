using Donutella.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donutella.Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneAsync(string phone);
    Task<User?> GetByResetTokenAsync(string token);
}