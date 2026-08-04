using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    // An order is made up of many finalized items
    public List<OrderItem> Items { get; set; } = new();
}