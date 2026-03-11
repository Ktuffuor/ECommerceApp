using System.Reflection.Metadata;
using Application.DTOs;
using Application.Interfaces;
using Common.CommonResponse;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<ApiResponse<ProductResponseDto>>
{
    public Guid Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequest<ProductResponseDto>
{
    private readonly IGenericRepository<ProductResponseDto> _repository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    public GetProductByIdQueryHandler(IGenericRepository<ProductResponseDto> repository, ILogger<GetProductByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<ProductResponseDto?> Handle(GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            FormattableString spc = $"spcGetProductById {request.Id}";
            var result = await _repository.GetAllAsync(spc);
            return result.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving product with Id {Id}", request.Id);
            throw;
        }
    }
    
}

