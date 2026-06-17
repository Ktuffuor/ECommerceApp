using Application.Interfaces;
using Common.CommonResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

// 1. The data expected from the client
public class LoginUserCommand : IRequest<ApiResponse<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// 2. The handler containing the business logic
public class LoginUserCommandHandler(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    ILogger<LoginUserCommandHandler> logger) 
    : IRequestHandler<LoginUserCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Retrieve the user by email
        var user = await repository.GetUserByEmailAsync(request.Email);
        
        if (user == null)
        {
            logger.LogWarning("Login failed: User with email {Email} not found.", request.Email);
            return new ApiResponse<string> 
            { 
                Success = false, 
                StatusCode = 401, 
                Message = "Invalid email or password.",
                Data = null
            };
        }

        // Verify the provided password against the BCrypt hash
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash!))
        {
            logger.LogWarning("Login failed: Invalid password for user {Email}.", request.Email);
            return new ApiResponse<string> 
            { 
                Success = false, 
                StatusCode = 401, 
                Message = "Invalid email or password.",
                Data = null
            };
        }

        // Check if they have verified their email
        if (!user.IsVerified)
        {
            logger.LogWarning("Login failed: User {Email} has not verified their account.", request.Email);
            return new ApiResponse<string> 
            { 
                Success = false, 
                StatusCode = 403, 
                Message = "Please verify your email address before logging in.",
                Data = null
            };
        }

        // Generate the JWT
        var token = jwtTokenGenerator.GenerateToken(user);
        
        logger.LogInformation("User {Email} logged in successfully.", request.Email);

        // Return the token inside the ApiResponse Data payload
        return new ApiResponse<string>
        {
            Success = true,
            StatusCode = 200,
            Message = "Login successful.",
            Data = token
        };
    }
}