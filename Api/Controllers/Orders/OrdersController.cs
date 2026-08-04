using Application.Features.Orders.Commands;
using Application.Features.Orders.Queries;
using Application.Interfaces.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Orders;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutCommand command)
    {
        if (!HasValidUser())
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        if (!HasValidUser())
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        var response = await mediator.Send(new GetMyOrdersQuery());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
    {
        if (!HasValidUser())
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        var response = await mediator.Send(new GetOrderByIdQuery { OrderId = orderId });
        return StatusCode(response.StatusCode, response);
    }

    private bool HasValidUser()
    {
        var userId = currentUserService.UserId;
        return userId != null && userId != Guid.Empty;
    }
}
