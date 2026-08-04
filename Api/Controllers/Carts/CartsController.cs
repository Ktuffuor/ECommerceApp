using System.Security.Claims;
using Application.Features.Carts.Commands;
using Application.Features.Carts.Queries;
using Application.Interfaces;
using Application.Interfaces.Users;
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
        var userId = currentUserService.UserId;

        if (userId == null || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }
        
        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("my-cart")]
    public async Task<IActionResult> GetMyCart()
    {
        var userId = currentUserService.UserId;

        if (userId == null || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        var query = new GetCartQuery {};
        var response = await mediator.Send(query);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete("remove/{productId:guid}")]
    public async Task<IActionResult> RemoveFromCart([FromRoute]Guid productId)
    {
        var userId = currentUserService.UserId;

        if (userId == null || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid or missing user token." });
        }

        var command = new RemoveFromCartCommand
        {
            ProductId = productId
        };

        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }
}