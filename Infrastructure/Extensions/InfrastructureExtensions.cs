using Application.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddRespositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, IProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return services;
    }
}