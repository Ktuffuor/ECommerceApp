using FluentValidation;

namespace Application.Features.Products.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("A valid Product ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("A valid Product Name is required")
            .MaximumLength(100).WithMessage("A valid Product Name cannot exceed 100 characters");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("A valid Price is required");
        
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("A valid StockQuantity is required");
    }
}