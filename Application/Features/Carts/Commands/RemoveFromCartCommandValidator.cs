using FluentValidation;

namespace Application.Features.Carts.Commands;

public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    public RemoveFromCartCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("A valid Product ID is required to remove an item.");
    }
}