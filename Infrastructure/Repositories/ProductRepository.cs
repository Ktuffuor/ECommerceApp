using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository (ECommerceDbContext context) : IProductRepository
{
    public async Task<Product> CreateProductAsync(
        string name,
        string description,
        decimal price,
        decimal stockQuantity
        )
    {
        FormattableString spc = $"EXEC spcCreateProduct {name}, {description}, {price}, {stockQuantity}";

        var results = await context.Products
            .FromSqlInterpolated(spc)
            .AsNoTracking()
            .FirstAsync();
        
        return results;
    }
}