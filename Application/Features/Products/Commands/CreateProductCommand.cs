using Application.DTOs.ProductDto;
using Application.Interfaces;
using Application.Interfaces.General;
using Application.Interfaces.Products;
using Common.CommonResponse;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<ApiResponse<ProductResponseDto>>
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductDesc { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int ProductStockQty { get; set; }
    public string ProductBrand { get; set; } = string.Empty;
}

public class CreateProductCommandHandler(IProductRepository repository, IValidator<CreateProductCommand> validator, IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, ApiResponse<ProductResponseDto>>
{
    public async Task<ApiResponse<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var createProduct = await repository.CreateProductAsync(
                request.ProductName,
                request.ProductDesc,
                request.ProductPrice,
                request.ProductStockQty,
                request.ProductBrand);

            if (createProduct == null)
            {
                logger.LogError("Database error: SPC failed to return the created product {ProductName}", request.ProductName);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return new ApiResponse<ProductResponseDto>
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "A database error occurred while creating the product.",
                    Data = null
                };
            }
            
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Successfully created product with ID: {ProductId}", createProduct.ProductId);
            
            return new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = 200,
                Message = "Product created successfully.",
                Data = mapper.Map<ProductResponseDto>(createProduct)
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during creating product {ProductName}", request.ProductName);
            await  unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        
    }
}