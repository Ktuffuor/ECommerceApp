using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<ProductResponseDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public class CreateProductCommandHandler(IProductRepository repository, IValidator<CreateProductCommand> validator)
    : IRequestHandler<CreateProductCommand, ProductResponseDto>
{
    public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var product = await repository.CreateProductAsync(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity);
        
        return product
    }
}