using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddRespositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, IProductRepository>();
        
        return services;
    }
}