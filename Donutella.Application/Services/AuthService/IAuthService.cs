using Donutella.Domain.Entities;

namespace Donutella.Application.Services.AuthService;

public interface IAuthService
{
    Task<(bool Success, string Token, string Message, User? User)> LoginAsync(string email, string password);
    Task<(bool Success, string Token, string Message, User? User)> RegisterAsync(string name, string email, string password, string phone = "");
    Task<bool> LogoutAsync(int userId);
    Task<(bool Success, string Message)> ForgotPasswordAsync(string email);
    Task<(bool Success, string Message)> ResetPasswordAsync(string token, string newPassword, string confirmPassword);
}
