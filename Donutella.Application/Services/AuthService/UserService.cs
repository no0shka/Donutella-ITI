using Donutella.Application.Interfaces;
using Donutella.Application.Services.AuthService;
using Donutella.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Donutella.Application.Services.AuthService;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Users.GetByIdAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _unitOfWork.Users.GetByEmailAsync(email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _unitOfWork.Users.GetAllAsync();
    }

    public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, string name, string phone)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return (false, "User not found.");

        if (string.IsNullOrWhiteSpace(name))
            return (false, "Name is required.");

        phone ??= string.Empty;
        if (!string.IsNullOrEmpty(phone))
        {
            var existingPhone = await _unitOfWork.Users.GetByPhoneAsync(phone);
            if (existingPhone != null && existingPhone.Id != userId)
                return (false, "Phone number is already in use.");
        }
        user.Name = name.Trim();
        user.Phone = phone.Trim();
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return (true, "Profile updated successfully.");
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return (false, "User not found.");

        if (!VerifyPassword(currentPassword, user.HashPassword))
            return (false, "Current password is incorrect.");

        user.HashPassword = HashPassword(newPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return (true, "Password changed successfully.");
    }

    public async Task<bool> DeleteAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return false;

        user.IsDeleted = true;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var computedHash = HashPassword(password);
        return computedHash == hash;
    }
}
