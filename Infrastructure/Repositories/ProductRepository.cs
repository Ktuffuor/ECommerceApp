using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository (ECommerceDbContext context) : IProductRepository
{
    public async Task<Product> CreateProductAsync(
        string Name,
        string Description,
        decimal Price,
        decimal StockQuantity
        )
    {
        FormattableString spc = $"EXEC spcCreateProduct {Name}, {Description}, {Price}, {StockQuantity}";

        var results = await context.Products
            .FromSqlInterpolated(spc)
            .AsNoTracking()
            .FirstAsync();
        
        return results;
    }
}