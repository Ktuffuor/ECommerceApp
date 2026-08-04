namespace Application.Interfaces.Users;

public interface IPasswordHasher
{
    string HashPassword(string password);
    
    bool VerifyPassword(string password, string passwordHash);
}