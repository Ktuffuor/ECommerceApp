using Domain.Entities;

namespace Application.Interfaces.Products;

public interface IProductRepository
{
    Task<Product?> CreateProductAsync(string productName, string productDesc, decimal productPrice, decimal productStockQty, string productBrand);
    Task<Product?> UpdateProductAsync(Guid productId, string productName, string productDesc, decimal productPrice, decimal productStockQty,  string productBrand);
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<List<Product>> GetAllProductsAsync(string? searchText, int pageNumber, int pageSize);
    Task<bool> HasActiveOrdersAsync(Guid productId);
    Task<bool> SoftDeleteProductAsync(Guid productId);
}