using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateProductAsync(string Name, string Description, decimal Price, decimal StockQuantity);
}