using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email;

public class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendEmailConfirmationAsync(
        string email,
        string firstName,
        string confirmationToken,
        CancellationToken cancellationToken = default)
    {
        var confirmationUrl = BuildConfirmationUrl(email, confirmationToken);

        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            logger.LogWarning(
                "SMTP host is not configured. Email confirmation link for {Email}: {ConfirmationUrl}",
                email,
                confirmationUrl);
            return;
        }

        var fromEmail = string.IsNullOrWhiteSpace(_options.FromEmail)
            ? _options.Username
            : _options.FromEmail;

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException("Email sender is missing Email:FromEmail or Email:Username configuration.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, _options.FromName),
            Subject = "Confirm your ECommerceApp account",
            Body = BuildEmailBody(firstName, confirmationUrl),
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(email));

        using var smtpClient = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.UseSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            smtpClient.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        await smtpClient.SendMailAsync(message, cancellationToken);
    }

    private string BuildConfirmationUrl(string email, string confirmationToken)
    {
        var baseUrl = _options.ConfirmationBaseUrl.TrimEnd('/');
        
        // EscapeDataString safely encodes special characters (like the @ symbol in emails)
        var encodedEmail = Uri.EscapeDataString(email);
        var encodedToken = Uri.EscapeDataString(confirmationToken);

        return $"{baseUrl}/api/users/verify-email?email={encodedEmail}&token={encodedToken}";
    }

    private static string BuildEmailBody(string firstName, string confirmationUrl)
    {
        var greetingName = WebUtility.HtmlEncode(firstName.Trim());
        var encodedUrl = WebUtility.HtmlEncode(confirmationUrl);

        return $"""
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2>Hello {greetingName},</h2>
                <p>Please confirm your email address by clicking the link below:</p>
                <p>
                    <a href="{encodedUrl}" style='display: inline-block; padding: 10px 20px; color: white; background-color: #007bff; text-decoration: none; border-radius: 5px;'>
                        Confirm email address
                    </a>
                </p>
                <p style='margin-top: 20px; font-size: 12px; color: gray;'>
                    This link expires in 24 hours. If the button doesn't work, copy and paste this link: <br/> {encodedUrl}
                </p>
            </div>
            """;
    }
}