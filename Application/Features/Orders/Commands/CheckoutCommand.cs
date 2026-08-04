using Application.Interfaces;
using Application.Interfaces.Carts;
using Application.Interfaces.General;
using Application.Interfaces.Payments;
using Application.Interfaces.Users;
using Common.CommonResponse;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands;

public class CheckoutCommand : IRequest<ApiResponse<string>>
{
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}

public class CheckoutCommandHandler(
    ICartRepository cartRepository,
    IOrderRepository orderRepository,
    IPaymentService paymentService,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    ILogger<CheckoutCommandHandler> logger) : IRequestHandler<CheckoutCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        // 1. Fetch and validate the cart
        var cart = await cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null || !cart.Items.Any())
        {
            return new ApiResponse<string> { Success = false, StatusCode = 400, Message = "Cart is empty." };
        }

        // 2. Calculate the Grand Total
        decimal totalAmount = cart.Items.Sum(i => i.Quantity * (i.Product?.ProductPrice ?? 0));

        // 3. Call the Mock External Payment Gateway (NO DB Transaction open yet!)
        var paymentRequest = new PaymentRequestDto
        {
            CardNumber = request.CardNumber,
            ExpiryDate = request.ExpiryDate,
            Cvv = request.Cvv,
            Amount = totalAmount
        };

        var paymentResult = await paymentService.ProcessPaymentAsync(paymentRequest);

        if (!paymentResult.Success)
        {
            logger.LogWarning("Payment failed for user {UserId}. Reason: {Message}", userId, paymentResult.Message);
            return new ApiResponse<string> { Success = false, StatusCode = 400, Message = paymentResult.Message };
        }

        // 4. Money Secured! Now we open the DB Transaction to save the Order and empty the Cart
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Paid,
                OrderDate = DateTime.UtcNow
            };

            // Snapshot the cart items into order items
            foreach (var cartItem in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product?.ProductPrice ?? 0
                });
                
                // Remove the item from the cart
                await cartRepository.RemoveCartItemAsync(cartItem);
            }

            await orderRepository.CreateOrderAsync(order);
            
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Order successfully created for user {UserId}. Txn: {TxnId}", userId, paymentResult.TransactionId);

            return new ApiResponse<string> 
            { 
                Success = true, 
                StatusCode = 200, 
                Message = "Checkout successful!", 
                Data = paymentResult.TransactionId 
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical database error saving order after successful payment for user {UserId}", userId);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            // In a real enterprise app, you would queue a background job here to refund the user since the DB save failed!
            throw; 
        }
    }
}