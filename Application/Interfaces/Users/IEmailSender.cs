namespace Application.Interfaces.Users;

public interface IEmailSender
{
    Task SendEmailConfirmationAsync(
        string email,
        string firstName,
        string confirmationToken,
        CancellationToken cancellationToken = default);
}
