using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Products.Commands;

namespace Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command )
    {
        var response = await mediator.Send(command);

        return StatusCode(response.StatusCode, response);
    
    }
}