using Donutella.Domain.Entities;

namespace Donutella.Application.Services.JWTService;

public interface IJwtService
{
    string GenerateToken(User user);
}
