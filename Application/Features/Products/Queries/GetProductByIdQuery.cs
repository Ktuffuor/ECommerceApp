using Application.DTOs;
using Application.DTOs.ProductDto;
using Application.Interfaces;
using Application.Interfaces.Products;
using Common.CommonResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<ApiResponse<ProductResponseDto>>
{
    public Guid ProductId { get; set; }
}

public class GetProductByIdQueryHandler(IProductRepository repository, ILogger<GetProductByIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductResponseDto>>
{
    public async Task<ApiResponse<ProductResponseDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetProductByIdAsync(request.ProductId);

            if (result != null)
                return new ApiResponse<ProductResponseDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = $"Product with id: {request.ProductId} retrieved successfully.",
                    Data = new ProductResponseDto
                    {
                        ProductId = result.ProductId,
                        ProductName = result.ProductName,
                        ProductDesc = result.ProductDesc,
                        ProductPrice = result.ProductPrice,
                        ProductStockQty = result.ProductStockQty,
                        ProductBrand = result.ProductBrand
                    }
                };
            logger.LogInformation("Product with Id {Id} was not found.", request.ProductId);
            return new ApiResponse<ProductResponseDto>
            {
                Success = false,
                StatusCode = 404,
                Message = $"Product with id: {request.ProductId} not found.",
                Data = null
            };

        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving product with Id {Id}", request.ProductId);
            
            return new ApiResponse<ProductResponseDto>
            {
                Success = false,
                StatusCode = 500,
                Message = "An internal server error occurred while processing your request.",
                Data = null
            };
        }
    }
}


