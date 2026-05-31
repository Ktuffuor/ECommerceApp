using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Common.CommonResponse;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;

namespace Application.Features.Products.Commands;

public class DeleteProductCommand : IRequest<ApiResponse<bool>>
{
    public Guid ProductId { get; set; }
}

public class DeleteProductCommandHander(IUnitOfWork unitOfWork, IValidator<DeleteProductCommand> validator, ILogger<DeleteProductCommandHander> logger, IProductRepository repository) : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to soft-delete a product of Id {ProductId}", request.ProductId);
        
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            logger.LogWarning("Validation failed for deleting product Id: {ProductId}",  request.ProductId);
            throw new ValidationException(validation.Errors);
        }
        
        var hasActiveOrders = await repository.HasActiveOrdersAsync(request.ProductId);
        if (hasActiveOrders)
        {
            logger.LogWarning("Cannot delete product Id {ProductId} because it has active orders", request.ProductId);
            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "This product cannot be deleted because it has active orders tied to it.",
                Data = false
            };
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var isDeleted = await repository.SoftDeleteProductAsync(request.ProductId);
            
            if (!isDeleted)
            {
                logger.LogWarning("Product with ID {ProductId} was not found or could not be deleted.", request.ProductId);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);

                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = 404, 
                    Message = $"Product with ID {request.ProductId} could not be found.",
                    Data = false
                };
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Successfully soft-deleted product with ID: {ProductId}", request.ProductId);

            return new ApiResponse<bool>
            {
                Success = true,
                StatusCode = 200,
                Message = "Product deleted successfully.",
                Data = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while deleting product ID: {ProductId}", request.ProductId);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}