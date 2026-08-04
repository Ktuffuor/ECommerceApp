using Application.DTOs.Orders;
using Application.Interfaces.Carts;
using Application.Interfaces.Users;
using Common.CommonResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries;

public class GetMyOrdersQuery : IRequest<ApiResponse<List<OrderResponseDto>>>
{
}

public class GetMyOrdersQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService,
    ILogger<GetMyOrdersQueryHandler> logger)
    : IRequestHandler<GetMyOrdersQuery, ApiResponse<List<OrderResponseDto>>>
{
    public async Task<ApiResponse<List<OrderResponseDto>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var orders = await orderRepository.GetOrdersByUserIdAsync(userId);

        logger.LogInformation("Retrieved {OrderCount} orders for user {UserId}", orders.Count, userId);

        return new ApiResponse<List<OrderResponseDto>>
        {
            Success = true,
            StatusCode = 200,
            Message = "Orders retrieved successfully.",
            Data = orders.Select(OrderMapper.ToDto).ToList()
        };
    }
}
