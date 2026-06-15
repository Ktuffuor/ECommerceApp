using Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Users;
[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterUserCommand command )
    {
        var response = await mediator.Send(command);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        var response = await mediator.Send(new ConfirmEmailCommand { Token = token });
        return StatusCode(response.StatusCode, response);
    }
}
