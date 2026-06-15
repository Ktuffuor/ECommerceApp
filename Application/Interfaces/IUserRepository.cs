using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(string email);
    Task<Guid> CreateUserAsync(User user);
    Task<User?> GetByEmailVerificationTokenHashAsync(string tokenHash);
    void UpdateUser(User user);
}
