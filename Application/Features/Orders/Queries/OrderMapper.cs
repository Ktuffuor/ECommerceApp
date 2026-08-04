using Application.DTOs.Orders;
using Domain.Entities;

namespace Application.Features.Orders.Queries;

internal static class OrderMapper
{
    public static OrderResponseDto ToDto(Order order)
    {
        return new OrderResponseDto
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = order.Items.Select(item => new OrderItemDto
            {
                OrderItemId = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.ProductName ?? "Unknown Product",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }
}
