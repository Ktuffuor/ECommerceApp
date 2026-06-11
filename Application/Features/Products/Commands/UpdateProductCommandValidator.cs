using FluentValidation;

namespace Application.Features.Products.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("A valid Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("A valid Product Name is required")
            .MaximumLength(100).WithMessage("A valid Product Name cannot exceed 100 characters");
        
        RuleFor(x => x.ProductPrice)
            .GreaterThan(0).WithMessage("A valid Price is required");
        
        RuleFor(x => x.ProductStockQty)
            .GreaterThanOrEqualTo(0).WithMessage("A valid StockQuantity is required");
    }
}