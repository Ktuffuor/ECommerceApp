using Application.Interfaces;
using Infrastructure.Email;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces.Carts;
using Application.Interfaces.General;
using Application.Interfaces.Payments;
using Application.Interfaces.Products;
using Application.Interfaces.Users;
using Infrastructure.Services;

namespace Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<Infrastructure.Security.JwtOptions>(config.GetSection(Infrastructure.Security.JwtOptions.SectionName));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IEmailSender, SesEmailSender>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddHttpClient<IPaymentService, PaymentService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5056"); 
    
            // Fail the request if the payment gateway takes more than 10 seconds to reply
            client.Timeout = TimeSpan.FromSeconds(10); 
        });
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is missing")))
                };
            });
        
        return services;
    }
}