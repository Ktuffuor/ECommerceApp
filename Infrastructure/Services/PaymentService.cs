using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces.Payments;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class PaymentService(HttpClient httpClient, ILogger<PaymentService> logger) : IPaymentService
{
    public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request)
    {
        try
        {
            logger.LogInformation("Attempting to process payment for amount: {Amount}", request.Amount);

            // Send the POST request to the Gateway
            var response = await httpClient.PostAsJsonAsync("/api/charge", request);

            // Read the JSON response back into our C# DTO
            var result = await response.Content.ReadFromJsonAsync<PaymentResponseDto>();

            return result ?? new PaymentResponseDto { Success = false, Message = "Unknown error from payment gateway." };
        }
        catch (HttpRequestException ex)
        {
            // This catches network timeouts or if the gateway is completely offline
            logger.LogError(ex, "Network error while reaching the payment gateway.");
            return new PaymentResponseDto { Success = false, Message = "Payment service is currently unavailable." };
        }
    }
}