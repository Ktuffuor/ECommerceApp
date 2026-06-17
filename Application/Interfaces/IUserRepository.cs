using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(string email);
    Task<Guid> CreateUserAsync(User user);
    Task<User?> GetByEmailVerificationTokenHashAsync(string tokenHash);
    Task<User?> GetUserByEmailAsync(string email);
    void UpdateUser(User user);
}
