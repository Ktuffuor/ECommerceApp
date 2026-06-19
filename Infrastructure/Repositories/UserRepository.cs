using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ECommerceDbContext context) : IUserRepository
{
    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();

        // We return true if NO user exists with this email address.
        return !await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email != null && u.Email.ToUpper() == normalizedEmail);
    }

    public async Task<Guid> CreateUserAsync(User user)
    {
        if (user.UserId == Guid.Empty)
        {
            user.UserId = Guid.NewGuid();
        }

        await context.Users.AddAsync(user);
        return user.UserId;
    }

    public async Task<User?> GetByEmailVerificationTokenHashAsync(string tokenHash)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationTokenHash == tokenHash);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        // FirstOrDefaultAsync will return the User if it finds an exact email match, 
        // or it will return null if the user doesn't exist.
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    

    public void UpdateUser(User user)
    {
        context.Users.Update(user);
    }
}
