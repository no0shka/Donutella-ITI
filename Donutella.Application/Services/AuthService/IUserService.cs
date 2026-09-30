using Donutella.Domain.Entities;

namespace Donutella.Application.Services.AuthService;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<(bool Success, string Message)> UpdateProfileAsync(int userId, string name, string phone);
    Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> DeleteAsync(int userId);
}