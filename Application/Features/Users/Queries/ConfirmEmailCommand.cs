using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Interfaces.General;
using Application.Interfaces.Users;
using Common.CommonResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Queries;

public class ConfirmEmailCommand : IRequest<ApiResponse<bool>>
{
    [JsonPropertyName("UserEmail")]
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class ConfirmEmailCommandHandler(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<ConfirmEmailCommandHandler> logger)
    : IRequestHandler<ConfirmEmailCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token))
        {
            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Both Email and Token are required.",
                Data = false
            };
        }

        var user = await repository.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            logger.LogWarning("Email confirmation failed because user {Email} was not found.", request.Email);
            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Invalid email or token.",
                Data = false
            };
        }

        if (user.IsVerified)
        {
            return new ApiResponse<bool>
            {
                Success = true,
                StatusCode = 200,
                Message = "Email address is already verified.",
                Data = true
            };
        }

        // Verify the raw token mathematically against the database hash
        if (string.IsNullOrEmpty(user.EmailVerificationTokenHash) || 
            !passwordHasher.VerifyPassword(request.Token, user.EmailVerificationTokenHash))
        {
            logger.LogWarning("Email confirmation failed: Invalid token for user {UserId}.", user.UserId);
            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Invalid email confirmation token.",
                Data = false
            };
        }

        if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Email confirmation token expired for user {UserId}.", user.UserId);
            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Email confirmation token has expired.",
                Data = false
            };
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            user.IsVerified = true;
            user.EmailVerificationTokenHash = null;
            user.EmailVerificationTokenExpiresAt = null;

            repository.UpdateUser(user); 
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Email verified successfully for user {UserId}.", user.UserId);
            return new ApiResponse<bool>
            {
                Success = true,
                StatusCode = 200,
                Message = "Email address verified successfully.",
                Data = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while confirming email for user {UserId}.", user.UserId);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}