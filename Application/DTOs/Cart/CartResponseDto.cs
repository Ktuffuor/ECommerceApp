namespace Application.DTOs.Cart;

public class CartItemDto
{
    public Guid ItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    
    // Auto-calculates the cost for this specific row
    public decimal SubTotal => UnitPrice * Quantity; 
}

public class CartResponseDto
{
    public Guid CartId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    
    // Auto-calculates the grand total for the entire cart
    public decimal TotalPrice => Items.Sum(i => i.SubTotal); 
}