namespace Application.Interfaces;

public interface IEmailSender
{
    Task SendEmailConfirmationAsync(
        string email,
        string firstName,
        string confirmationToken,
        CancellationToken cancellationToken = default);
}
