using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Common.CommonResponse;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands;

public class UpdateProductCommand : IRequest<ApiResponse<ProductResponseDto>>
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductDesc { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int ProductStockQty { get; set; }
    public string ProductBrand { get; set; }
}

public class UpdateProductCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IProductRepository repository, ILogger<UpdateProductCommandHandler> logger, IValidator<UpdateProductCommand> validator) : IRequestHandler<UpdateProductCommand, ApiResponse<ProductResponseDto>>
{
    public async Task<ApiResponse<ProductResponseDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to update a product with ID: {ProductId}", request.ProductId);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for updating product ID: {ProductId}", request.ProductId);
            throw new ValidationException(validationResult.Errors);
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var updateProduct = await repository.UpdateProductAsync(
                request.ProductId,
                request.ProductName,
                request.ProductDesc,
                request.ProductPrice,
                request.ProductStockQty,
                request.ProductBrand);

            if (updateProduct == null)
            {
                logger.LogError("Database error: SPC failed to return the update product {ProductId}", request.ProductId);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);

                return new ApiResponse<ProductResponseDto>
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Product not found or a database error occurred while updating.",
                    Data = mapper.Map<ProductResponseDto>(updateProduct)
                };
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Successfully updated product of Id {ProductId}", request.ProductId);

            return new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = 200,
                Message = "Product successfully updated",
                Data = null
            };
        } 
        catch (Exception e)
        {
            logger.LogError(e, "Error during updating product {ProductId}", request.ProductId);
            await  unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}