using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository (ECommerceDbContext context) : IProductRepository
{
    public async Task<Product?> CreateProductAsync(string name, string description, decimal price, decimal stockQuantity, string brand)
    {
        var result = await context.Products
            .FromSqlInterpolated($"EXEC spcCreateProduct {name}, {description}, {price}, {stockQuantity}")
            .ToListAsync(); 

        return result.FirstOrDefault();
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        var result = await context.Products
            .FromSqlInterpolated($"EXEC spcGetProductById {productId}").ToListAsync();
        return result.FirstOrDefault();
    }
    
    public async Task<Product?> GetAllProductsAsync(string? searchText, int pageNumber, int pageSize)
    {
        var result = await context.Products
            .FromSqlInterpolated($"EXEC spcGetAllProducts {searchText}, {pageNumber}, {pageSize}").ToListAsync();
        return result.FirstOrDefault();
    }

    public async Task<Product?> UpdateProductAsync(Guid productId, string name, string description, decimal price, decimal stockQuantity, string brand)
    {
        var result = await context.Products
            .FromSqlInterpolated($"Exec spcUpdateProduct @productId={productId}, @productName={name}, @productDesc={description}, @productPrice={price}, @productStockQty={stockQuantity},  @productBrand={brand}").ToListAsync();
        return result.FirstOrDefault();
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