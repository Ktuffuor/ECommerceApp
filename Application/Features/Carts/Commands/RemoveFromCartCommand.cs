using Application.Interfaces;
using Application.Interfaces.Carts;
using Application.Interfaces.General;
using Application.Interfaces.Users;
using Common.CommonResponse;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Carts.Commands;

public class RemoveFromCartCommand : IRequest<ApiResponse<bool>>
{
    public Guid ProductId { get; set; }
}

public class RemoveFromCartCommandHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    IValidator<RemoveFromCartCommand> validator,
    ILogger<RemoveFromCartCommandHandler> logger) : IRequestHandler<RemoveFromCartCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Fetch the user's cart
            var cart = await cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return new ApiResponse<bool> { Success = false, StatusCode = 404, Message = "Cart not found." };
            }

            // 2. Locate the specific item inside the cart
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (itemToRemove == null)
            {
                return new ApiResponse<bool> { Success = false, StatusCode = 404, Message = "Item not found in cart." };
            }

            // 3. Remove it and commit
            await cartRepository.RemoveCartItemAsync(itemToRemove);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Successfully removed product {ProductId} from cart for user {UserId}", request.ProductId, userId);

            return new ApiResponse<bool> { Success = true, StatusCode = 200, Message = "Item successfully removed from cart.", Data = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing product {ProductId} from cart for user {UserId}", request.ProductId, userId);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}