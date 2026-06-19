using Application.DTOs.ProductDto;
using Application.Interfaces;
using Common.CommonResponse;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries;

public class GetAllProductsQuery : IRequest<ApiResponse<GetAllProductsDto>>
{
    public string? SearchText { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetAllProductsQueryHandler(IProductRepository repository, ILogger<GetAllProductsQueryHandler> logger)
    : IRequestHandler<GetAllProductsQuery, ApiResponse<GetAllProductsDto>>
{
    public async Task<ApiResponse<GetAllProductsDto>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetAllProductsAsync(request.SearchText, request.PageNumber, request.PageSize);

            if (result != null)
                return new ApiResponse<GetAllProductsDto>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Products were successfully retrieved.",
                    Data = result.Adapt<GetAllProductsDto>()
                };

            logger.LogInformation("No products were found.");
            return new ApiResponse<GetAllProductsDto>
            {
                Success = false,
                StatusCode = 404,
                Message = "No products were found.",
                Data = null
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving products");

            return new ApiResponse<GetAllProductsDto>
            {
                Success = false,
                StatusCode = 500,
                Message = "An internal server error occurred while processing your request.",
                Data = null
            };
        }
    }
}
