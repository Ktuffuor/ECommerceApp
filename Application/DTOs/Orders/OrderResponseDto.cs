using Domain.Enums;

namespace Application.DTOs.Orders;

public class OrderItemDto
{
    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal => UnitPrice * Quantity;
}

public class OrderResponseDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}
