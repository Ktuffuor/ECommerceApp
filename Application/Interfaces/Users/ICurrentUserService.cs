namespace Application.Interfaces.Users;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
}