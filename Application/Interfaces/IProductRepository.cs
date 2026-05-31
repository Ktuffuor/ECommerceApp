using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateProductAsync(string name, string description, decimal price, decimal stockQuantity);
    Task<Product> UpdateProductAsync(Guid productId, string name, string description, decimal price, decimal stockQuantity);
    Task<bool> HasActiveOrdersAsync(Guid productId);
    Task<bool> SoftDeleteProductAsync(Guid productId);
}