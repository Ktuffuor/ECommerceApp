using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CartRepository(ECommerceDbContext context) : ICartRepository
{
    public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
    {
        return await context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product) // Fetches the actual product details for the cart
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task CreateCartAsync(Cart cart)
    {
        await context.Carts.AddAsync(cart);
    }

    public async Task AddCartItemAsync(CartItem item)
    {
        await context.CartItems.AddAsync(item);
    }

    public Task RemoveCartItemAsync(CartItem item)
    {
        context.CartItems.Remove(item);
        return Task.CompletedTask;
    }
}