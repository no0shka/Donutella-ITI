using Donutella.Application.Interfaces;
using Donutella.Application.Services.AuthService;
using Donutella.Application.Services.JWTService;
using Donutella.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Donutella.Application.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<(bool Success, string Token, string Message, User? User)> LoginAsync(string email, string password)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        
        if (user == null)
            return (false, "", "البريد الإلكتروني أو كلمة المرور غير صحيحة", null);

        if (!VerifyPassword(password, user.HashPassword))
            return (false, "", "البريد الإلكتروني أو كلمة المرور غير صحيحة", null);

        var token = _jwtService.GenerateToken(user);
        return (true, token, "تم تسجيل الدخول بنجاح", user);
    }

    public async Task<(bool Success, string Token, string Message, User? User)> RegisterAsync(string name, string email, string password, string phone = "")
    {
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(email);
        if (existingUser != null)
            return (false, "", "البريد الإلكتروني مستخدم بالفعل", null);

        if (!string.IsNullOrEmpty(phone))
        {
            var existingPhone = await _unitOfWork.Users.GetByPhoneAsync(phone);
            if (existingPhone != null)
                return (false, "", "رقم الهاتف مستخدم بالفعل", null);
        }

        var user = new User
        {
            Name = name,
            Email = email,
            Phone = phone ?? string.Empty,
            HashPassword = HashPassword(password),
            Role = Donutella.Domain.Enums.RoleType.Customer
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        return (true, token, "تم إنشاء الحساب بنجاح", user);
    }

    public async Task<bool> LogoutAsync(int userId)
    {
        return true;
    }

    public async Task<(bool Success, string Message)> ForgotPasswordAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        
        if (user == null)
        {
            return (true, "إذا كان البريد الإلكتروني مسجلاً، ستتلقى رابط إعادة تعيين كلمة المرور");
        }

        var resetToken = GenerateResetToken();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        await SendResetEmailAsync(user.Email, resetToken);

        return (true, "إذا كان البريد الإلكتروني مسجلاً، ستتلقى رابط إعادة تعيين كلمة المرور");
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(string token, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
            return (false, "كلمة المرور الجديدة غير متطابقة");

        if (string.IsNullOrEmpty(token))
            return (false, "رمز إعادة التعيين غير صالح");

        var user = await _unitOfWork.Users.GetByResetTokenAsync(token);
        
        if (user == null)
            return (false, "رمز إعادة التعيين غير صالح أو منتهي الصلاحية");

        if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return (false, "رمز إعادة التعيين منتهي الصلاحية، يرجى طلب رابط جديد");

        user.HashPassword = HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return (true, "تم إعادة تعيين كلمة المرور بنجاح، يمكنك الآن تسجيل الدخول");
    }

    private string GenerateResetToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private Task SendResetEmailAsync(string email, string token)
    {
        var resetLink = $"https://localhost:5001/Account/ResetPassword?token={token}";
        Console.WriteLine($"=== PASSWORD RESET EMAIL ===");
        Console.WriteLine($"To: {email}");
        Console.WriteLine($"Reset Link: {resetLink}");
        Console.WriteLine($"Token: {token}");
        Console.WriteLine($"=============================");
        return Task.CompletedTask;
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
