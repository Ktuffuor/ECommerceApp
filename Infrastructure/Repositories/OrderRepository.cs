using Application.Interfaces.Carts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository(ECommerceDbContext context) : IOrderRepository
{
    public async Task CreateOrderAsync(Order order)
    {
        await context.Orders.AddAsync(order);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(Guid? userId)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId, Guid? userId)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
    }
}
