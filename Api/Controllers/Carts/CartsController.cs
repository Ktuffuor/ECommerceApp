using System.Security.Claims;
using Application.Features.Carts.Commands;
using Application.Features.Carts.Queries;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Carts;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartsController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command)
    {
        var userIdString = currentUserService.UserId;

        // 2. Try to parse it into an actual Guid
        if (!Guid.TryParse(userIdString, out var userId) || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        // 3. Securely override the UserId in the command (now that it is a real Guid!)
        command.UserId = userId;

        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("my-cart")]
    public async Task<IActionResult> GetMyCart()
    {
        var userIdString = currentUserService.UserId;

        if (!Guid.TryParse(userIdString, out var userId) || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        var query = new GetCartQuery { UserId = userId };
        var response = await mediator.Send(query);
        
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete("remove/{productId:guid}")]
    public async Task<IActionResult> RemoveFromCart([FromRoute]Guid productId)
    {
        var userIdString = currentUserService.UserId;

        if (!Guid.TryParse(userIdString, out var userId) || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        var command = new RemoveFromCartCommand
        {
            UserId = userId,
            ProductId = productId
        };

        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }
}