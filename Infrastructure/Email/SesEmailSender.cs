using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Application.Interfaces.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Email;

public class SesEmailSender(IConfiguration config, ILogger<SesEmailSender> logger) : IEmailSender
{
    public async Task SendEmailConfirmationAsync(string email, string firstName, string confirmationToken, CancellationToken cancellationToken)
    {
        var accessKey = config["AwsSettings:AccessKey"];
        var secretKey = config["AwsSettings:SecretKey"];
        var regionString = config["AwsSettings:Region"] ?? "us-east-1";
        var senderEmail = config["AwsSettings:VerifiedEmail"];
        
        var region = RegionEndpoint.GetBySystemName(regionString);
        var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
        var clientConfig = new AmazonSimpleEmailServiceConfig
        {
            RegionEndpoint = region
        };

        using var client = new AmazonSimpleEmailServiceClient(credentials, clientConfig);
        
        // The React frontend URL we will build next
        var verificationLink = $"http://localhost:5173/verify-email?token={Uri.EscapeDataString(confirmationToken)}&email={Uri.EscapeDataString(email)}";

        var sendRequest = new SendEmailRequest
        {
            Source = senderEmail,
            Destination = new Destination { ToAddresses = [email] },
            Message = new Message
            {
                Subject = new Content("Verify your E-Commerce Account"),
                Body = new Body
                {
                    Html = new Content($@"
                        <h2>Welcome to the Store, {firstName}!</h2>
                        <p>Please verify your email address by clicking the link below:</p>
                        <a href='{verificationLink}'>Verify Email</a>
                    "),
                    Text = new Content($"Hi {firstName}, verify your email here: {verificationLink}")
                }
            }
        };

        try
        {
            var response = await client.SendEmailAsync(sendRequest, cancellationToken);
            logger.LogInformation("Email sent successfully via Amazon SES. Message ID: {MessageId}", response.MessageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email via Amazon SES.");
            throw;
        }
    }
}