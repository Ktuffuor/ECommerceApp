using MediatR;

namespace Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<Product>
{
    public int Id { get; set; }
}
public class GetProductByIdQueryHandler

