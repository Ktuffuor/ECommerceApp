using Application.DTOs;
using Application.Interfaces;
using Common.CommonResponse;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<ApiResponse<ProductResponseDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
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
            var response = await repository.CreateProductAsync(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity);

            if (response == null)
            {
                logger.LogError("Database error: SPC failed to return the created product {ProductName}", request.Name);
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
            logger.LogInformation("Successfully created product with ID: {ProductId}", response.Id);
            
            return new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = 200,
                Message = "Product created successfully.",
                Data = mapper.Map<ProductResponseDto>(response)
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during creating product {ProductName}", request.Name);
            await  unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        
    }
}