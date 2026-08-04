using Domain.Entities;

namespace Application.Interfaces.Carts;

public interface ICartRepository
{
    // Retrieves the cart and all nested items/products
    Task<Cart?> GetCartByUserIdAsync(Guid? userId);
    
    Task CreateCartAsync(Cart cart);
    Task AddCartItemAsync(CartItem item);
    Task RemoveCartItemAsync(CartItem item);
}