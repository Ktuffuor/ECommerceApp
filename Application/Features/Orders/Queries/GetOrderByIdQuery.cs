using Application.DTOs.Orders;
using Application.Interfaces.Carts;
using Application.Interfaces.Users;
using Common.CommonResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries;

public class GetOrderByIdQuery : IRequest<ApiResponse<OrderResponseDto>>
{
    public Guid OrderId { get; set; }
}

public class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService,
    ILogger<GetOrderByIdQueryHandler> logger)
    : IRequestHandler<GetOrderByIdQuery, ApiResponse<OrderResponseDto>>
{
    public async Task<ApiResponse<OrderResponseDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var order = await orderRepository.GetOrderByIdAsync(request.OrderId, userId);

        if (order == null)
        {
            logger.LogInformation("Order {OrderId} was not found for user {UserId}", request.OrderId, userId);
            return new ApiResponse<OrderResponseDto>
            {
                Success = false,
                StatusCode = 404,
                Message = "Order not found.",
                Data = null
            };
        }

        return new ApiResponse<OrderResponseDto>
        {
            Success = true,
            StatusCode = 200,
            Message = "Order retrieved successfully.",
            Data = OrderMapper.ToDto(order)
        };
    }
}
