using Domain.Entities;

namespace Application.Interfaces.Users;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}