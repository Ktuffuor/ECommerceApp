using System.Text.Json.Serialization;

namespace Application.DTOs;

public class UserResponseDto
{
    public Guid UserId { get; set; } 
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    [JsonIgnore]
    public string? PasswordHash { get; set; }
    [JsonIgnore]
    public string? RefreshToken { get; set; }
    [JsonIgnore]
    public DateTime RefreshTokenExpiry { get; set; }
    public bool IsVerified { get; set; } = false;
    public string Role { get; set; } = "Customer"; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
