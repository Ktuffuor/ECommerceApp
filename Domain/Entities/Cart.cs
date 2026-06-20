namespace Domain.Entities;

public class Cart
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; } 
    public User? User { get; set; }
    
    public List<CartItem> Items { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}