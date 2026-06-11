using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Products.Commands;
using Application.Features.Products.Queries;

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
    
    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command )
    {
        var response = await mediator.Send(command);

        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductById([FromQuery] GetProductByIdQuery query)
    {
        var response = await mediator.Send(query);
        return StatusCode(response.StatusCode, response);
    }
}