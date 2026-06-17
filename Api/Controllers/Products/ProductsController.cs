using Application.Features.Products.Commands;
using Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Products;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command )
    {
        var response = await mediator.Send(command);

        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut("UpdateProduct")]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command )
    {
        var response = await mediator.Send(command);

        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid productId)
    {
        var response = await mediator.Send(new GetProductByIdQuery { ProductId = productId });
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("GetAllProducts")]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery query)
    {
        var response = await mediator.Send(query);
        return StatusCode(response.StatusCode, response);
    }
}