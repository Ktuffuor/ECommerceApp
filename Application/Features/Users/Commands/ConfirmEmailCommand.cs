using Application.Features.Users;
using Application.Interfaces;
using Common.CommonResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public class ConfirmEmailCommand : IRequest<ApiResponse<bool>>
{
    public string Token { get; set; } = string.Empty;
}

public class ConfirmEmailCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<ConfirmEmailCommandHandler> logger)
    : IRequestHandler<ConfirmEmailCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Email confirmation token is required.",
                Data = false
            };
        }

        var tokenHash = EmailVerificationToken.Hash(request.Token.Trim());
        var user = await repository.GetByEmailVerificationTokenHashAsync(tokenHash);

        if (user == null)
        {
            logger.LogWarning("Email confirmation failed because token was invalid.");
            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Invalid email confirmation token.",
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

            logger.LogInformation("Email verified for user {UserId}.", user.UserId);
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
