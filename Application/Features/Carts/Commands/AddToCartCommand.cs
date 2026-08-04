using Application.Interfaces;
using Application.Interfaces.Carts;
using Application.Interfaces.General;
using Application.Interfaces.Products;
using Application.Interfaces.Users;
using Common.CommonResponse;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Carts.Commands;

public class AddToCartCommand : IRequest<ApiResponse<bool>>
{
    // The UserId will be extracted securely from the JWT, not the frontend payload
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class AddToCartCommandHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    IValidator<AddToCartCommand> validator,
    ILogger<AddToCartCommandHandler> logger) : IRequestHandler<AddToCartCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Verify product and stock availability
            var product = await productRepository.GetProductByIdAsync(request.ProductId);
            if (product == null || product.IsDeleted)
            {
                return new ApiResponse<bool> { Success = false, StatusCode = 404, Message = "Product not found." };
            }

            /*if (product.ProductStockQty < request.Quantity)
            {
                return new ApiResponse<bool> { Success = false, StatusCode = 400, Message = "Insufficient stock available." };
            }*/

            // 2. Fetch or create the Cart
            var cart = await cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await cartRepository.CreateCartAsync(cart);
                
                // Save immediately so the new Cart gets an ID generated before we add items to it
                await unitOfWork.SaveChangesAsync(cancellationToken); 
            }

            // 3. Update existing item or add new item
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await cartRepository.AddCartItemAsync(newItem);
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Successfully added product {ProductId} to cart for user {UserId}", request.ProductId, userId);

            return new ApiResponse<bool> { Success = true, StatusCode = 200, Message = "Product added to cart successfully.", Data = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding product to cart for user {UserId}", userId);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}