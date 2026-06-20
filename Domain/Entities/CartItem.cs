namespace Domain.Entities;

public class CartItem
{
    public Guid Id { get; set; }
    
    // Links back to the parent Cart
    public Guid CartId { get; set; }
    public Cart? Cart { get; set; }
    
    // Links to your existing Product entity
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    public int Quantity { get; set; }
}