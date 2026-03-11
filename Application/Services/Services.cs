using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;

namespace Application.Services;

public static class Services
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddAutoMapper(assembly);
        
        return services;
    }
}