using FluentValidation;

namespace Application.Features.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");
        
        RuleFor(v => v.ProductName)
            .MaximumLength(250).WithMessage("Product description must not exceed 250 characters.");

        RuleFor(v => v.ProductPrice)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        
        RuleFor(v => v.ProductStockQty)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

    }
}