using Application.Features.Users.Queries;
using Application.Interfaces;
using Common.CommonResponse;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public class RegisterUserCommand : IRequest<ApiResponse<Guid>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserCommandHandler(
    IUserRepository repository,
    IEmailSender emailSender,
    IPasswordHasher passwordHasher,
    IValidator<RegisterUserCommand> validator,
    ILogger<RegisterUserCommandHandler> logger,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterUserCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to register new user with email: {Email}", request.Email);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var email = request.Email.Trim();
        var isEmailUnique = await repository.IsEmailUniqueAsync(email);
        if (!isEmailUnique)
        {
            logger.LogWarning("Registration failed. Email {Email} is already in use.", email);
            return new ApiResponse<Guid>
            {
                Success = false,
                StatusCode = 400,
                Message = "This email is already registered.",
                Data = Guid.Empty
            };
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var verificationToken = EmailVerificationToken.Generate();
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = email,
                PasswordHash = passwordHasher.HashPassword(request.Password),
                IsVerified = false,
                EmailVerificationTokenHash = passwordHasher.HashPassword(verificationToken),
                EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24),
                Role = "Customer"
            };

            var createdUserId = await repository.CreateUserAsync(user);
            await emailSender.SendEmailConfirmationAsync(
                email,
                request.FirstName,
                verificationToken,
                cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Successfully registered user with ID: {UserId}", createdUserId);

            return new ApiResponse<Guid>
            {
                Success = true,
                StatusCode = 201,
                Message = "User registered successfully. Please check your email to verify your account.",
                Data = createdUserId
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred during registration for email: {Email}", email);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
