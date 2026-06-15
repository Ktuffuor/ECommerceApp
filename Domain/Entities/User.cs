namespace Domain.Entities;

public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public bool IsVerified { get; set; } = false;
    public string? EmailVerificationTokenHash { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }
    public string Role { get; set; } = "Customer"; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
