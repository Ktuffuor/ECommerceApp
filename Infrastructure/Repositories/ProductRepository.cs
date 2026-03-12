using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository (ECommerceDbContext context) : IProductRepository
{
    public async Task<Product> CreateProductAsync(string name, string description, decimal price, decimal stockQuantity)
    {
        FormattableString spc = $"EXEC spcCreateProduct @Name={name}, @Description={description}, @Price={price}, @StockQuantity={stockQuantity}";

        var response = await context.Products
            .FromSqlInterpolated(spc)
            .AsNoTracking()
            .FirstAsync();
        
        return response;
    }

    public async Task<Product> UpdateProductAsync(Guid productId, string name, string description, decimal price, decimal stockQuantity)
    {
        FormattableString spc = $"Exec spcUpdateProduct @ProductId={productId}, @Name={name}, @Description={description}, @Price={price}, @StockQuantity={stockQuantity}";

        var response = await context.Products
            .FromSqlInterpolated(spc)
            .AsNoTracking()
            .FirstAsync();
        
        return response;
    }

    public async Task<bool> HasActiveOrdersAsync(Guid productId)
    {
        return await Task.FromResult(false);
    }

    public async Task<bool> SoftDeleteProductAsync(Guid productId)
    {
        var product = await context.Products.FindAsync(productId);

        if (product == null || product.IsDeleted)
        {
            return false;
        }

        product.IsDeleted = true;
        
        context.Products.Update(product);

        return true;
    }
}