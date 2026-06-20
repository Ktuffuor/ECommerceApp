using Application.DTOs;
using Application.DTOs.Cart;
using Application.Interfaces;
using Common.CommonResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Carts.Queries;

public class GetCartQuery : IRequest<ApiResponse<CartResponseDto>>
{
    public Guid UserId { get; set; }
}

public class GetCartQueryHandler(ICartRepository cartRepository, ILogger<GetCartQueryHandler> logger) 
    : IRequestHandler<GetCartQuery, ApiResponse<CartResponseDto>>
{
    public async Task<ApiResponse<CartResponseDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetCartByUserIdAsync(request.UserId);

        if (cart == null)
        {
            // If they don't have a cart yet, return an empty one instead of an error
            return new ApiResponse<CartResponseDto>
            {
                Success = true,
                StatusCode = 200,
                Data = new CartResponseDto()
            };
        }

        // Map the Entity to the DTO manually (or use AutoMapper if you prefer)
        var cartDto = new CartResponseDto
        {
            CartId = cart.Id,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ItemId = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.ProductName ?? "Unknown Product",
                UnitPrice = i.Product?.ProductPrice ?? 0,
                Quantity = i.Quantity
            }).ToList()
        };

        logger.LogInformation("Successfully retrieved cart for user {UserId}", request.UserId);

        return new ApiResponse<CartResponseDto>
        {
            Success = true,
            StatusCode = 200,
            Data = cartDto
        };
    }
}