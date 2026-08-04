using Domain.Entities;

namespace Application.Interfaces.Carts;

public interface IOrderRepository
{
    Task CreateOrderAsync(Order order);
    Task<List<Order>> GetOrdersByUserIdAsync(Guid? userId);
    Task<Order?> GetOrderByIdAsync(Guid orderId, Guid? userId);
}
